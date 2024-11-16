using System;
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
            ModelInfo = builder.ModelInfo;
            AmbientLoopingSoundFormID = builder.AmbientLoopingSoundFormID;
        }
    }
    
    // ReSharper disable once InconsistentNaming
    public class MSTTBuilder
    {
        public Record BaseInfo;
        public string EditorID;
        public Model ModelInfo;
        public uint AmbientLoopingSoundFormID;
        
        private MSTTBuilder() {}
        
        public static MSTTBuilder CreateAndConfigure(Action<MSTTBuilder> configurator)
        {
            var builder = new MSTTBuilder();
            configurator(builder);
            return builder;
        }
        
        public MSTT Build()
        {
            return new MSTT(this);
        }
    }
}