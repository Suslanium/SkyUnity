using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Manager.Extensions
{
    public class MasterFileManagerLoadingScreenExtension : MasterFileManagerExtension
    {
        private const string LoadingScreenRecordType = "LSCR";
        private readonly Random _random = new(DateTime.Now.Millisecond);

        public MasterFileManagerLoadingScreenExtension(MasterFileManager masterFileManager) : base(masterFileManager) {}

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public LSCR GetRandomLoadingScreen()
        {
            MasterFileManager.MasterFilesInitialization.Wait();

            var masterFileToLoadingScreenFormIds = MasterFileManager.ReverseLoadOrder
                .Where(fileName =>
                    MasterFileManager.MasterFiles[fileName].ContainsRecordsOfType(LoadingScreenRecordType))
                .Select(fileName => (MasterFileManager.MasterFiles[fileName],
                    MasterFileManager.MasterFiles[fileName].RecordTypeToFormIdToPosition[LoadingScreenRecordType].Keys))
                .ToList();
            
            var loadingScreenAmount = masterFileToLoadingScreenFormIds.Sum(x => x.Keys.Count());
            var randomLoadingScreenIndex = _random.Next(loadingScreenAmount);
            
            foreach (var (masterFile, formIds) in masterFileToLoadingScreenFormIds)
            {
                var currentFileLoadingScreenAmount = formIds.Count();
                if (randomLoadingScreenIndex < currentFileLoadingScreenAmount)
                {
                    return masterFile.GetFromFormId<LSCR>(formIds.ElementAt(randomLoadingScreenIndex));
                }

                randomLoadingScreenIndex -= currentFileLoadingScreenAmount;
            }

            return null;
        }
    }
}