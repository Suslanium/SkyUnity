using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Common;
using Core.Common.DI;
using Core.MasterFile.Parser.Extensions;
using Core.MasterFile.Parser.Reader;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser
{
    public class MasterFile : IDisposable
    {
        private readonly Dictionary<uint, long> _formIdToPosition = new();
        private readonly Dictionary<uint, Group> _formIdToParentGroup = new();
        private readonly Dictionary<string, long> _recordTypeToGroupPosition = new();
        private readonly Dictionary<string, Dictionary<uint, long>> _recordTypeToFormIdToPosition = new();
        private readonly ConcurrentDictionary<uint, Record> _recordCache = new();

        private readonly MasterFileReader _reader;
        private readonly BinaryReader _fileReader;
        private readonly Task _initializationTask;

        private readonly IReadOnlyDictionary<Type, IMasterFileExtension> _extensions;

        public readonly TES4 FileHeader;
        public readonly MasterFileProperties Properties;
        public readonly string FileName;
        public bool IsInitialized => _initializationTask.IsCompleted;

        public IReadOnlyDictionary<uint, long> FormIdToPosition => _formIdToPosition;
        public IReadOnlyDictionary<uint, Group> FormIdToParentGroup => _formIdToParentGroup;
        public IReadOnlyDictionary<string, long> RecordTypeToGroupPosition => _recordTypeToGroupPosition;
        public readonly IReadOnlyDictionary<string, IReadOnlyDictionary<uint, long>> RecordTypeToFormIdToPosition;

        public MasterFile(string fileName, BinaryReader fileReader, MasterFileReader reader,
            IFactory<IReadOnlyCollection<IMasterFileExtension>> extensionsFactory)
        {
            FileName = fileName;
            _fileReader = fileReader;
            _reader = reader;
            RecordTypeToFormIdToPosition =
                new CovariantReadOnlyDictionary<string, Dictionary<uint, long>, IReadOnlyDictionary<uint, long>>(
                    _recordTypeToFormIdToPosition);
            _extensions = extensionsFactory.Create().ToDictionary(extension => extension.GetType());
            
            FileHeader = _reader.ReadEntry(MasterFileProperties.DummyInstance, _fileReader, 0) as TES4;
            Properties = MasterFileProperties.FromTES4(FileHeader);
            _initializationTask = Task.Run(Initialize);
        }

        private void Initialize()
        {
            Stack<(Group, long)> groupStack = new();
            while (_fileReader.BaseStream.Position < _fileReader.BaseStream.Length)
            {
                Group currentGroup = null;
                // Determine the current group via going through the stack
                // until the current stream position is within the group's data
                if (groupStack.Count > 0)
                {
                    var groupDataEndPosition = groupStack.Peek().Item2;
                    while (_fileReader.BaseStream.Position >= groupDataEndPosition)
                    {
                        groupStack.Pop();
                        if (groupStack.Count == 0)
                            break;
                        groupDataEndPosition = groupStack.Peek().Item2;
                    }

                    if (groupStack.Count > 0)
                    {
                        currentGroup = groupStack.Peek().Item1;
                    }
                }

                var entryStartPosition = _fileReader.BaseStream.Position;
                var entry = _reader.ReadEntryHeader(_fileReader, _fileReader.BaseStream.Position);
                switch (entry)
                {
                    case Record record:
                        _formIdToPosition.Add(record.FormId, entryStartPosition);
                        _formIdToParentGroup.Add(record.FormId, currentGroup);

                        if (!_recordTypeToFormIdToPosition.TryGetValue(record.Type, out var value))
                        {
                            _recordTypeToFormIdToPosition.Add(
                                record.Type,
                                new Dictionary<uint, long> { { record.FormId, entryStartPosition } });
                        }
                        else
                        {
                            value.Add(record.FormId, entryStartPosition);
                        }

                        foreach (var extension in _extensions.Values)
                        {
                            extension.Initializer.OnRecordHeaderParsed(entryStartPosition, record, currentGroup);
                        }

                        break;
                    case Group group:
                        //GroupType 0 is a top-level group containing records of a specific type
                        if (group.GroupType == 0)
                        {
                            var groupRecordsType = System.Text.Encoding.UTF8.GetString(group.Label);
                            _recordTypeToGroupPosition[groupRecordsType] = entryStartPosition;
                        }
                        
                        foreach (var extension in _extensions.Values)
                        {
                            extension.Initializer.OnGroupHeaderParsed(entryStartPosition, group, currentGroup);
                        }

                        groupStack.Push((group, entryStartPosition + group.Size - 24));
                        break;
                }
            }
            
            foreach (var extension in _extensions.Values)
            {
                extension.FinishInitialization(_fileReader, _reader, this);
            }
        }

        public async Task AwaitInitialization()
        {
            await _initializationTask;
        }
        
        public TExtension GetExtension<TExtension>() where TExtension : IMasterFileExtension
        {
            if (!_extensions.TryGetValue(typeof(TExtension), out var extension))
            {
                throw new ArgumentException($"MasterFile extension of type {typeof(TExtension)} not found.");
            }

            return (TExtension) extension;
        }

        public void EnsureInitialized()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Master file is not initialized yet.");
            }
        }

        public T GetFromFormId<T>(uint formID) where T : Record
        {
            EnsureInitialized();
            if (_recordCache.TryGetValue(formID, out var cachedRecord))
            {
                return (T) cachedRecord;
            }
            
            if (!_formIdToPosition.TryGetValue(formID, out var position))
            {
                return null;
            }

            T record;
            lock (_fileReader)
            {
                record = _reader.ReadEntry(Properties, _fileReader, position) as T;
            }
            _recordCache[formID] = record;
            return record;
        }

        public MasterFileEntry ReadAfterRecord(Record record)
        {
            EnsureInitialized();
            if (!_formIdToPosition.TryGetValue(record.FormId, out var position))
            {
                return null;
            }

            lock (_fileReader)
            {
                //Skip the found record before reading the following entry
                _reader.ReadEntryHeader(_fileReader, position);

                return _reader.ReadEntry(Properties, _fileReader, _fileReader.BaseStream.Position);
            }
        }

        public bool RecordExists(uint formID)
        {
            EnsureInitialized();
            return _formIdToPosition.ContainsKey(formID);
        }

        public bool ContainsRecordsOfType(string recordType)
        {
            EnsureInitialized();
            return _recordTypeToFormIdToPosition.ContainsKey(recordType);
        }

        /// <summary>
        /// For the record stored in the World Children/Cell (Persistent/Temporary) Children/Topic Children Group,
        /// find the FormID of their parent WRLD/CELL/DIAL
        /// </summary>
        /// <returns>
        /// The formID of the parent record, or 0 if the record does not exist
        /// or is not stored in one of the groups specified above
        /// </returns>
        public uint GetRecordParentFormId(uint formID)
        {
            EnsureInitialized();
            if (!_formIdToParentGroup.TryGetValue(formID, out var group))
            {
                return 0;
            }

            //World Children/Cell (Persistent/Temporary) Children/Topic Children group types
            return group.GroupType is not 1 and not 6 and not 7 and not 8 and not 9
                ? 0
                : BitConverter.ToUInt32(group.Label);
        }

        public void Dispose()
        {
            _initializationTask?.Dispose();
            _fileReader?.Dispose();
        }
    }
}