using Core.MasterFile.Parser.Structures.Records.Builder;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.Model;

namespace Core.MasterFile.Parser.Structures.Records
{
    /// <summary>
    /// TREE records contain information on trees as well other flora that can be activated.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TREE : Record
    {
        public readonly string EditorID;
        
        public readonly Model ModelInfo;
        
        public readonly float TrunkFlexibility;
        
        public readonly float BranchFlexibility;
        
        public readonly float LeafFrequency;
        
        public readonly float LeafAmplitude;
        
        public TREE(TREEBuilder builder) : base(builder.BaseInfo)
        {
            EditorID = builder.EditorID;
            ModelInfo = builder.ModelInfo.Build();
            TrunkFlexibility = builder.TrunkFlexibility;
            BranchFlexibility = builder.BranchFlexibility;
            LeafFrequency = builder.LeafFrequency;
            LeafAmplitude = builder.LeafAmplitude;
        }
    }
    
    // ReSharper disable once InconsistentNaming
    public class TREEBuilder : IRecordBuilder
    {
        public string EditorID;
        public ModelBuilder ModelInfo = new();
        public float TrunkFlexibility;
        public float BranchFlexibility;
        public float LeafFrequency;
        public float LeafAmplitude;
        
        public Record BaseInfo { get; set; }
        
        public Record Build()
        {
            return new TREE(this);
        }
    }
}