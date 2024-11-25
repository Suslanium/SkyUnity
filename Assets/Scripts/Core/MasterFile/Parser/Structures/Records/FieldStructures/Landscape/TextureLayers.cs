namespace Core.MasterFile.Parser.Structures.Records.FieldStructures.Landscape
{
    public class AdditionalTextureLayer
    {
        public readonly uint LandTextureFormID;

        /// <summary>
        /// 0 - bottom left;
        /// 1 - bottom right;
        /// 2 - upper left;
        /// 3 - upper right;
        /// </summary>
        public readonly byte Quadrant;

        public readonly ushort LayerIndex;

        public readonly float[,] QuadrantAlphaMap;

        public AdditionalTextureLayer(uint landTextureFormID, byte quadrant, ushort layerIndex,
            float[,] quadrantAlphaMap)
        {
            LandTextureFormID = landTextureFormID;
            Quadrant = quadrant;
            LayerIndex = layerIndex;
            QuadrantAlphaMap = quadrantAlphaMap;
        }
    }

    public class BaseTextureLayer
    {
        public readonly uint LandTextureFormID;

        /// <summary>
        /// 0 - bottom left;
        /// 1 - bottom right;
        /// 2 - upper left;
        /// 3 - upper right;
        /// </summary>
        public readonly byte Quadrant;

        public BaseTextureLayer(uint landTextureFormID, byte quadrant)
        {
            LandTextureFormID = landTextureFormID;
            Quadrant = quadrant;
        }
    }
}