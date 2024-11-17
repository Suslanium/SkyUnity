using Core.MasterFile.Parser.Structures.Records.Builder;

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
    public class LTEXBuilder : IRecordBuilder
    {
        public string EditorID;
        public uint TextureFormID;
        
        public Record BaseInfo { get; set; }
        
        public Record Build()
        {
            return new LTEX(this);
        }
    }
}