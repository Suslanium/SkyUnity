using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Common;
using Core.MasterFile.Parser.Reader;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser
{
    public class MasterFile
    {
        private readonly Dictionary<uint, long> _formIdToPosition = new();
        private readonly Dictionary<uint, Group> _formIdToParentGroup = new();
        private readonly Dictionary<string, long> _recordTypeToGroupPosition = new();
        private readonly Dictionary<string, Dictionary<uint, long>> _recordTypeToFormIdToPosition = new();

        private readonly MasterFileReader _parser;
        private readonly BinaryReader _fileReader;
        private readonly Task _initializationTask;

        public readonly TES4 FileHeader;
        public readonly MasterFileProperties Properties;
        public bool IsInitialized => _initializationTask.IsCompleted;

        public IReadOnlyDictionary<uint, long> FormIdToPosition => _formIdToPosition;
        public IReadOnlyDictionary<uint, Group> FormIdToParentGroup => _formIdToParentGroup;
        public IReadOnlyDictionary<string, long> RecordTypeToGroupPosition => _recordTypeToGroupPosition;
        public readonly IReadOnlyDictionary<string, IReadOnlyDictionary<uint, long>> RecordTypeToFormIdToPosition;

        public MasterFile(BinaryReader fileReader, MasterFileReader parser)
        {
            _fileReader = fileReader;
            _parser = parser;
            RecordTypeToFormIdToPosition = 
                new CovariantReadOnlyDictionary<string, Dictionary<uint, long>, IReadOnlyDictionary<uint, long>>(_recordTypeToFormIdToPosition);
            FileHeader = _parser.ReadEntry(MasterFileProperties.DummyInstance, _fileReader, 0) as TES4;
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
                        currentGroup = groupStack.Peek().Item1;
                }

                var entryStartPosition = _fileReader.BaseStream.Position;
                var entry = _parser.ReadEntryHeader(_fileReader, _fileReader.BaseStream.Position);
                switch (entry)
                {
                    case Record record:
                        _formIdToPosition.Add(record.FormID, entryStartPosition);
                        _formIdToParentGroup.Add(record.FormID, currentGroup);

                        if (!_recordTypeToFormIdToPosition.TryGetValue(record.Type, out var value))
                        {
                            _recordTypeToFormIdToPosition.Add(
                                record.Type,
                                new Dictionary<uint, long> { { record.FormID, entryStartPosition } });
                        }
                        else
                        {
                            value.Add(record.FormID, entryStartPosition);
                        }
                        //TODO additional logic for modules (ex. cell/wrld records handling)

                        break;
                    case Group group:
                        //GroupType 0 is a top-level group containing records of a specific type
                        if (group.GroupType == 0)
                        {
                            var groupRecordsType = System.Text.Encoding.UTF8.GetString(group.Label);
                            _recordTypeToGroupPosition.Add(groupRecordsType, entryStartPosition);
                        }
                        //TODO additional logic for modules (ex. cell group handling)

                        groupStack.Push((group, entryStartPosition + group.Size - 24));
                        break;
                }
            }
        }
        
        public async Task AwaitInitialization()
        {
            await _initializationTask;
        }
        
        public void EnsureInitialized()
        {
            if (!IsInitialized)
                throw new System.InvalidOperationException("Master file is not initialized yet.");
        }
        
        public Record GetFromFormID(uint formID)
        {
            EnsureInitialized();
            if (!_formIdToPosition.TryGetValue(formID, out var position))
                return null;
            
            lock (_fileReader)
            {
                return _parser.ReadEntry(Properties, _fileReader, position) as Record;
            }
        }

        public MasterFileEntry ReadAfterRecord(Record record)
        {
            EnsureInitialized();
            if (!_formIdToPosition.TryGetValue(record.FormID, out var position))
                return null;

            lock (_fileReader)
            {
                //Skip the found record before reading the following entry
                _parser.ReadEntryHeader(_fileReader, position);
                
                return _parser.ReadEntry(Properties, _fileReader, _fileReader.BaseStream.Position);
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
        /// The formID of the parent record, or 0 if the record doesnot exist
        /// or is not stored in one of the groups specified above
        /// </returns>
        public uint GetRecordParentFormID(uint formID)
        {
            EnsureInitialized();
            if (!_formIdToParentGroup.TryGetValue(formID, out var group))
                return 0;
            
            return group.GroupType is not 1 and not 6 and not 7 and not 8 and not 9
                ? 0
                : BitConverter.ToUInt32(group.Label);
        }
    }
}