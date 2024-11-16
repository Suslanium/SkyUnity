using System;
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
    public class LGTMBuilder
    {
        public Record BaseInfo;
        public string EditorID;
        public Lighting LightingInfo;
        
        private LGTMBuilder() {}
        
        public static LGTMBuilder CreateAndConfigure(Action<LGTMBuilder> configurator)
        {
            var builder = new LGTMBuilder();
            configurator(builder);
            return builder;
        }
        
        public LGTM Build()
        {
            return new LGTM(this);
        }
    }
}