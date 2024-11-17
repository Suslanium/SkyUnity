using System.Collections.Generic;
using Core.MasterFile.Parser.Structures.Records.Builder;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.General;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.Landscape;

namespace Core.MasterFile.Parser.Structures.Records
{
    /// <summary>
    /// Exterior cell land data (terrain) record.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class LAND : Record
    {
        /// <summary>
        /// Land vertex height map in game units.
        /// </summary>
        public readonly float[,] VertexHeightMap;

        public readonly ColorRGB[,] VertexColors;

        public readonly IReadOnlyList<BaseTextureLayer> BaseTextures;

        public readonly IReadOnlyList<AdditionalTextureLayer> AdditionalTextures;
        
        public LAND(LANDBuilder builder) : base(builder.BaseInfo)
        {
            VertexHeightMap = builder.VertexHeightMap;
            VertexColors = builder.VertexColors;
            BaseTextures = builder.BaseTextures;
            AdditionalTextures = builder.AdditionalTextures;
        }
    }

    // ReSharper disable once InconsistentNaming
    public class LANDBuilder : IRecordBuilder
    {
        public float[,] VertexHeightMap;
        public ColorRGB[,] VertexColors;
        public List<BaseTextureLayer> BaseTextures = new();
        public List<AdditionalTextureLayer> AdditionalTextures = new();
        
        public Record BaseInfo { get; set; }
        
        public Record Build()
        {
            return new LAND(this);
        }
    }
}