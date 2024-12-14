using System.Runtime.CompilerServices;
using Core.MasterFile.Common.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Manager.Extensions
{
    public static class ExtensionHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LSCR GetRandomLoadingScreen(this MasterFileManager manager)
        {
            return manager.GetExtension<MasterFileManagerLoadingScreenExtension>().GetRandomLoadingScreen();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CELL FindCellByEditorId(this MasterFileManager manager, string editorId)
        {
            return manager.GetExtension<MasterFileManagerCellExtension>().FindCellByEditorId(editorId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetWorldSpaceFormId(this MasterFileManager manager, uint cellFormId)
        {
            return manager.GetExtension<MasterFileManagerCellExtension>().GetWorldSpaceFormId(cellFormId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawCellData GetWorldSpacePersistentCellData(this MasterFileManager manager, uint worldSpaceFormId)
        {
            return manager.GetExtension<MasterFileManagerCellExtension>()
                .GetWorldSpacePersistentCellData(worldSpaceFormId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawCellData GetCellData(this MasterFileManager manager, uint cellFormId)
        {
            return manager.GetExtension<MasterFileManagerCellExtension>().GetCellData(cellFormId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawCellData GetExteriorCellData(this MasterFileManager manager, FullCellPosition cellPosition)
        {
            return manager.GetExtension<MasterFileManagerCellExtension>().GetExteriorCellData(cellPosition);
        }
    }
}