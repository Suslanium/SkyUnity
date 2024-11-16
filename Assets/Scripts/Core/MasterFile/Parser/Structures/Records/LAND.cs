using System;
using System.Collections.Generic;
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

        public readonly List<BaseTextureLayer> BaseTextures;

        public readonly List<AdditionalTextureLayer> AdditionalTextures;
        
        public LAND(LANDBuilder builder) : base(builder.BaseInfo)
        {
            VertexHeightMap = builder.VertexHeightMap;
            VertexColors = builder.VertexColors;
            BaseTextures = builder.BaseTextures;
            AdditionalTextures = builder.AdditionalTextures;
        }
    }

    // ReSharper disable once InconsistentNaming
    public class LANDBuilder
    {
        public Record BaseInfo;
        public float[,] VertexHeightMap;
        public ColorRGB[,] VertexColors;
        public List<BaseTextureLayer> BaseTextures = new();
        public List<AdditionalTextureLayer> AdditionalTextures = new();
        
        private LANDBuilder() {}

        public static LANDBuilder CreateAndConfigure(Action<LANDBuilder> configurator)
        {
            var builder = new LANDBuilder();
            configurator(builder);
            return builder;
        }
        
        public LAND Build()
        {
            return new LAND(this);
        }
    }
}