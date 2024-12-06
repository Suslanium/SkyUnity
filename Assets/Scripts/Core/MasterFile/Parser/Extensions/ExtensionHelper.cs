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

        public static CELL GetExteriorCellByGridPosition(this MasterFile masterFile, uint worldSpaceFormId,
            short blockX, short blockY, short subBlockX, short subBlockY, int xGridPosition, int yGridPosition)
        {
            return masterFile.GetExtension<CellExtension>().GetExteriorCellByGridPosition(worldSpaceFormId, blockX,
                blockY, subBlockX, subBlockY, xGridPosition, yGridPosition);
        }

        public static CELL GetPersistentWorldSpaceCell(this MasterFile masterFile, uint worldSpaceFormId)
        {
            return masterFile.GetExtension<CellExtension>().GetPersistentWorldSpaceCell(worldSpaceFormId);
        }
    }
}