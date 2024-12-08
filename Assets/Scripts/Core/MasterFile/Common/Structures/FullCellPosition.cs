namespace Core.MasterFile.Common.Structures
{
    public readonly struct FullCellPosition
    {
        public readonly short BlockX;
        public readonly short BlockY;
        public readonly short SubBlockX;
        public readonly short SubBlockY;
        public readonly int GridPositionX;
        public readonly int GridPositionY;
        public readonly uint WorldSpaceFormId;
        
        public FullCellPosition(short blockX, short blockY, short subBlockX, short subBlockY, int gridPositionX,
            int gridPositionY, uint worldSpaceFormId)
        {
            BlockX = blockX;
            BlockY = blockY;
            SubBlockX = subBlockX;
            SubBlockY = subBlockY;
            GridPositionX = gridPositionX;
            GridPositionY = gridPositionY;
            WorldSpaceFormId = worldSpaceFormId;
        }
    }
}