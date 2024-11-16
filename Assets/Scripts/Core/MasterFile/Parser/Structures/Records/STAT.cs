using System;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.Model;

namespace Core.MasterFile.Parser.Structures.Records
{
    /// <summary>
    /// STAT records contain information on static objects.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class STAT : Record
    {
        public readonly string EditorID;
        
        public readonly Model ModelInfo;
        
        /// <summary>
        /// MaxAngle 30-120 degrees
        /// </summary>
        public readonly float DirMaterialMaxAngle;

        /// <summary>
        /// Directional Material - MATO formID
        /// </summary>
        public readonly uint DirectionalMaterialFormID;
        
        public STAT(STATBuilder builder) : base(builder.BaseInfo)
        {
            EditorID = builder.EditorID;
            ModelInfo = builder.ModelInfo;
            DirMaterialMaxAngle = builder.DirMaterialMaxAngle;
            DirectionalMaterialFormID = builder.DirectionalMaterialFormID;
        }
    }
    
    // ReSharper disable once InconsistentNaming
    public class STATBuilder
    {
        public Record BaseInfo;
        public string EditorID;
        public Model ModelInfo;
        public float DirMaterialMaxAngle;
        public uint DirectionalMaterialFormID;
        
        private STATBuilder() {}
        
        public static STATBuilder CreateAndConfigure(Action<STATBuilder> configurator)
        {
            var builder = new STATBuilder();
            configurator(builder);
            return builder;
        }
        
        public STAT Build()
        {
            return new STAT(this);
        }
    }
}