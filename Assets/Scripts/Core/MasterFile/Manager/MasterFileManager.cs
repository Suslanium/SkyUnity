using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.DI;
using Core.MasterFile.Common.Structures;
using Core.MasterFile.Manager.Extensions;
using Core.MasterFile.Parser.Structures;

namespace Core.MasterFile.Manager
{
    public class MasterFileManager : IDisposable
    {
        public static string FilePathArgumentName = "MasterFilePath";
        public static string FileNameArgumentName = "MasterFileName";
        
        private readonly Dictionary<string, Parser.MasterFile> _masterFiles = new();
        private readonly List<string> _reverseLoadOrder = new();
        private readonly Dictionary<Type, MasterFileManagerExtension> _extensions = new();
        private readonly ConcurrentDictionary<uint, Record> _recordCache = new();
        private bool _masterFilesAreInitialized;

        public IReadOnlyDictionary<string, Parser.MasterFile> MasterFiles => _masterFiles;
        public IReadOnlyList<string> ReverseLoadOrder => _reverseLoadOrder;
        public readonly Task MasterFilesInitialization;

        public MasterFileManager(
            IReadOnlyList<string> masterFilePaths,
            IFactory<Parser.MasterFile> masterFileProvider,
            IFactory<IReadOnlyCollection<MasterFileManagerExtension>> masterFileManagerExtensionsFactory)
        {
            var loadOrder = masterFilePaths.Select(Path.GetFileName).ToArray();
            for (byte loadOrderIndex = 0; loadOrderIndex < masterFilePaths.Count; loadOrderIndex++)
            {
                var filePath = masterFilePaths[loadOrderIndex];
                var fileName = Path.GetFileName(filePath);
                var loadOrderInfo = new LoadOrderInfo(loadOrder, loadOrderIndex);
                var masterFile = masterFileProvider.Create(configurator =>
                {
                    configurator.SetNamedArgument(FilePathArgumentName, filePath);
                    configurator.SetNamedArgument(FileNameArgumentName, fileName);
                    configurator.SetArgument(loadOrderInfo);
                });
                
                var missingMasters = masterFile.Properties.FileMasters
                    .Where(masterName => !_masterFiles.ContainsKey(masterName)).ToList();
                if (missingMasters.Count > 0)
                {
                    throw new FileNotFoundException(
                        $"Incorrect load order: masterfile(s) {string.Join(',', missingMasters)} " +
                        $"required for {masterFile.Properties.FileName} not found.");
                }

                _masterFiles.Add(masterFile.Properties.FileName, masterFile);
                _reverseLoadOrder.Insert(0, masterFile.Properties.FileName);
            }
            
            var masterFileManagerExtensions = 
                masterFileManagerExtensionsFactory.Create(configurator =>
                {
                    configurator.SetArgument(this);
                });
            foreach (var extension in masterFileManagerExtensions)
            {
                _extensions.Add(extension.GetType(), extension);
            }
            
            MasterFilesInitialization = Task.Run(async () =>
            {
                if (_masterFilesAreInitialized)
                {
                    return;
                }
                
                await Task.WhenAll(MasterFiles.Values.Select(masterFile => masterFile.AwaitInitialization()));
                _masterFilesAreInitialized = true;
            });
        }
        
        public TExtension GetExtension<TExtension>() where TExtension : MasterFileManagerExtension
        {
            if (!_extensions.TryGetValue(typeof(TExtension), out var extension))
            {
                throw new KeyNotFoundException($"MasterFile manager extension of type {typeof(TExtension)} not found.");
            }
            
            return (TExtension) extension;
        }

        public T GetFromFormId<T>(uint formId) where T : Record
        {
            if (_recordCache.TryGetValue(formId, out var cachedRecord))
            {
                return (T) cachedRecord;
            }
            
            MasterFilesInitialization.Wait();

            var masterFileName = 
                ReverseLoadOrder.FirstOrDefault(fileName => MasterFiles[fileName].RecordExists(formId));
            var record = masterFileName == null ? null : MasterFiles[masterFileName].GetFromFormId<T>(formId);
            _recordCache[formId] = record;
            return record;
        }

        /// <summary>
        /// For the record stored in the World Children/Cell (Persistent/Temporary) Children/Topic Children Group,
        /// find the FormID of their parent WRLD/CELL/DIAL
        /// </summary>
        /// <returns>
        /// The formID of the parent record, or 0 if the record does not exist
        /// or is not stored in one of the groups specified above
        /// </returns>
        public uint GetRecordParentFormId(uint recordFormId)
        {
            MasterFilesInitialization.Wait();

            var masterFileName = 
                ReverseLoadOrder.FirstOrDefault(fileName => MasterFiles[fileName].RecordExists(recordFormId));
            return masterFileName == null ? 0 : MasterFiles[masterFileName].GetRecordParentFormId(recordFormId);
        }

        public void Dispose()
        {
            foreach (var masterFile in _masterFiles.Values)
            {
                masterFile.Dispose();
            }
        }
    }
}