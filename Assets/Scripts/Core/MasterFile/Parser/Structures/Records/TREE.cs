using System;
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
            ModelInfo = builder.ModelInfo;
            TrunkFlexibility = builder.TrunkFlexibility;
            BranchFlexibility = builder.BranchFlexibility;
            LeafFrequency = builder.LeafFrequency;
            LeafAmplitude = builder.LeafAmplitude;
        }
    }
    
    // ReSharper disable once InconsistentNaming
    public class TREEBuilder
    {
        public Record BaseInfo;
        public string EditorID;
        public Model ModelInfo;
        public float TrunkFlexibility;
        public float BranchFlexibility;
        public float LeafFrequency;
        public float LeafAmplitude;
        
        private TREEBuilder() {}
        
        public static TREEBuilder CreateAndConfigure(Action<TREEBuilder> configurator)
        {
            var builder = new TREEBuilder();
            configurator(builder);
            return builder;
        }
        
        public TREE Build()
        {
            return new TREE(this);
        }
    }
}