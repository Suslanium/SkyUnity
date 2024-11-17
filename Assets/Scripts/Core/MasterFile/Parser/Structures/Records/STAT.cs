using Core.MasterFile.Parser.Structures.Records.Builder;
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
            ModelInfo = builder.ModelInfo.Build();
            DirMaterialMaxAngle = builder.DirMaterialMaxAngle;
            DirectionalMaterialFormID = builder.DirectionalMaterialFormID;
        }
    }
    
    // ReSharper disable once InconsistentNaming
    public class STATBuilder : IRecordBuilder
    {
        public string EditorID;
        public ModelBuilder ModelInfo = new();
        public float DirMaterialMaxAngle;
        public uint DirectionalMaterialFormID;
        
        public Record BaseInfo { get; set; }
        
        public Record Build()
        {
            return new STAT(this);
        }
    }
}