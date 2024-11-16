using System;

namespace Core.MasterFile.Parser.Structures.Records
{
    // ReSharper disable once InconsistentNaming
    public class TXST : Record
    {
        public readonly string EditorID;

        public readonly string DiffuseMapPath;

        public readonly string NormalMapPath;

        public readonly string MaskMapPath;

        public readonly string GlowMapPath;

        public readonly string DetailMapPath;

        public readonly string EnvironmentMapPath;

        public readonly string MultiLayerMapPath;

        public readonly string SpecularMapPath;

        /// <summary>
        /// flags:
        /// 0x01 - not Has specular map;
        /// 0x02 - Facegen Textures;
        /// 0x04 - Has model space normal map;
        /// </summary>
        public readonly ushort Flags;
        
        public TXST(TXSTBuilder builder) : base(builder.BaseInfo)
        {
            EditorID = builder.EditorID;
            DiffuseMapPath = builder.DiffuseMapPath;
            NormalMapPath = builder.NormalMapPath;
            MaskMapPath = builder.MaskMapPath;
            GlowMapPath = builder.GlowMapPath;
            DetailMapPath = builder.DetailMapPath;
            EnvironmentMapPath = builder.EnvironmentMapPath;
            MultiLayerMapPath = builder.MultiLayerMapPath;
            SpecularMapPath = builder.SpecularMapPath;
            Flags = builder.Flags;
        }
    }
    
    // ReSharper disable once InconsistentNaming
    public class TXSTBuilder
    {
        public Record BaseInfo;
        public string EditorID;
        public string DiffuseMapPath;
        public string NormalMapPath;
        public string MaskMapPath;
        public string GlowMapPath;
        public string DetailMapPath;
        public string EnvironmentMapPath;
        public string MultiLayerMapPath;
        public string SpecularMapPath;
        public ushort Flags;

        private TXSTBuilder() {}
        
        public static TXSTBuilder CreateAndConfigure(Action<TXSTBuilder> configurator)
        {
            var builder = new TXSTBuilder();
            configurator(builder);
            return builder;
        }
        
        public TXST Build()
        {
            return new TXST(this);
        }
    }
}