using Core.MasterFile.Common.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser.Extensions
{
    public static class ExtensionHelper
    {
        public static CELL FindCellByEditorId(this MasterFile masterFile, string editorId)
        {
            return masterFile.GetExtension<CellExtension>().FindCellByEditorId(editorId);
        }

        public static uint GetWorldSpaceFormId(this MasterFile masterFile, uint cellFormId)
        {
            return masterFile.GetExtension<CellExtension>().GetWorldSpaceFormId(cellFormId);
        }

        public static CELL GetExteriorCellByPosition(this MasterFile masterFile, FullCellPosition cellPosition)
        {
            return masterFile.GetExtension<CellExtension>().GetExteriorCellByPosition(cellPosition);
        }

        public static CELL GetPersistentWorldSpaceCell(this MasterFile masterFile, uint worldSpaceFormId)
        {
            return masterFile.GetExtension<CellExtension>().GetPersistentWorldSpaceCell(worldSpaceFormId);
        }
    }
}