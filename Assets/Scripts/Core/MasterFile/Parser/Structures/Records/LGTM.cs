using Core.MasterFile.Parser.Structures.Records.Builder;
using Core.MasterFile.Parser.Structures.Records.FieldStructures;

namespace Core.MasterFile.Parser.Structures.Records
{
    /// <summary>
    /// LGTM records contain information on lighting templates.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class LGTM : Record
    {
        public readonly string EditorID;

        public readonly Lighting LightingInfo;
        
        public LGTM(LGTMBuilder builder) : base(builder.BaseInfo)
        {
            EditorID = builder.EditorID;
            LightingInfo = builder.LightingInfo;
        }
    }

    // ReSharper disable once InconsistentNaming
    public class LGTMBuilder : IRecordBuilder
    {
        public string EditorID;
        public Lighting LightingInfo;
        
        public Record BaseInfo { get; set; }
        
        public Record Build()
        {
            return new LGTM(this);
        }
    }
}