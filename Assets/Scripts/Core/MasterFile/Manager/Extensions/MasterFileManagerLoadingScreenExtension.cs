using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Manager.Extensions
{
    public class MasterFileManagerLoadingScreenExtension : MasterFileManagerExtension
    {
        private const string LoadingScreenRecordType = "LSCR";
        private readonly Random _random = new(DateTime.Now.Millisecond);
        private List<uint> _loadingScreenFormIds;

        public MasterFileManagerLoadingScreenExtension(MasterFileManager masterFileManager) : base(masterFileManager)
        {
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public LSCR GetRandomLoadingScreen()
        {
            MasterFileManager.MasterFilesInitialization.Wait();

            _loadingScreenFormIds ??= MasterFileManager.ReverseLoadOrder
                .Where(fileName =>
                    MasterFileManager.MasterFiles[fileName].ContainsRecordsOfType(LoadingScreenRecordType))
                .SelectMany(fileName =>
                    MasterFileManager.MasterFiles[fileName].RecordTypeToFormIdToPosition[LoadingScreenRecordType].Keys)
                .ToList();

            var randomLoadingScreenIndex = _random.Next(_loadingScreenFormIds.Count);
            if (randomLoadingScreenIndex < 0 || randomLoadingScreenIndex >= _loadingScreenFormIds.Count)
            {
                return null;
            }

            var randomLoadingScreenFormId = _loadingScreenFormIds[randomLoadingScreenIndex];
            return MasterFileManager.GetFromFormId<LSCR>(randomLoadingScreenFormId);
        }
    }
}