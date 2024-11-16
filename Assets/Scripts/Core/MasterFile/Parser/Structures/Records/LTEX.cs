using System;

namespace Core.MasterFile.Parser.Structures.Records
{
    /// <summary>
    /// Land texture record
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class LTEX : Record
    {
        public readonly string EditorID;
        
        /// <summary>
        /// TXST FormID
        /// </summary>
        public readonly uint TextureFormID;
        
        public LTEX(LTEXBuilder builder) : base(builder.BaseInfo)
        {
            EditorID = builder.EditorID;
            TextureFormID = builder.TextureFormID;
        }
    }

    // ReSharper disable once InconsistentNaming
    public class LTEXBuilder
    {
        public Record BaseInfo;
        public string EditorID;
        public uint TextureFormID;
        
        private LTEXBuilder() {}
        
        public static LTEXBuilder CreateAndConfigure(Action<LTEXBuilder> configurator)
        {
            var builder = new LTEXBuilder();
            configurator(builder);
            return builder;
        }
        
        public LTEX Build()
        {
            return new LTEX(this);
        }
    }
}