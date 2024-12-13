using Core.MasterFile.Common.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Manager.Extensions
{
    public static class ExtensionHelper
    {
        public static LSCR GetRandomLoadingScreen(this MasterFileManager manager)
        {
            return manager.GetExtension<MasterFileManagerLoadingScreenExtension>().GetRandomLoadingScreen();
        }

        public static CELL FindCellByEditorId(this MasterFileManager manager, string editorId)
        {
            return manager.GetExtension<MasterFileManagerCellExtension>().FindCellByEditorId(editorId);
        }

        public static uint GetWorldSpaceFormId(this MasterFileManager manager, uint cellFormId)
        {
            return manager.GetExtension<MasterFileManagerCellExtension>().GetWorldSpaceFormId(cellFormId);
        }

        public static RawCellData GetWorldSpacePersistentCellData(this MasterFileManager manager, uint worldSpaceFormId)
        {
            return manager.GetExtension<MasterFileManagerCellExtension>()
                .GetWorldSpacePersistentCellData(worldSpaceFormId);
        }

        public static RawCellData GetCellData(this MasterFileManager manager, uint cellFormId)
        {
            return manager.GetExtension<MasterFileManagerCellExtension>().GetCellData(cellFormId);
        }

        public static RawCellData GetExteriorCellData(this MasterFileManager manager, FullCellPosition cellPosition)
        {
            return manager.GetExtension<MasterFileManagerCellExtension>().GetExteriorCellData(cellPosition);
        }
    }
}