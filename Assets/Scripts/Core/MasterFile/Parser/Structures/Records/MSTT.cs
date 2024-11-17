using Core.MasterFile.Parser.Structures.Records.Builder;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.Model;

namespace Core.MasterFile.Parser.Structures.Records
{
    /// <summary>
    /// MSTT records contain information on movable static objects.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class MSTT : Record
    {
        public readonly string EditorID;

        public readonly Model ModelInfo;
        
        /// <summary>
        /// SNDR FormID
        /// </summary>
        public readonly uint AmbientLoopingSoundFormID;
        
        public MSTT(MSTTBuilder builder) : base(builder.BaseInfo)
        {
            EditorID = builder.EditorID;
            ModelInfo = builder.ModelInfo.Build();
            AmbientLoopingSoundFormID = builder.AmbientLoopingSoundFormID;
        }
    }
    
    // ReSharper disable once InconsistentNaming
    public class MSTTBuilder : IRecordBuilder
    {
        public string EditorID;
        public ModelBuilder ModelInfo = new();
        public uint AmbientLoopingSoundFormID;
        
        public Record BaseInfo { get; set; }
        
        public Record Build()
        {
            return new MSTT(this);
        }
    }
}