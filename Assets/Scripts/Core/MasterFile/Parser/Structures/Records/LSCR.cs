using Core.MasterFile.Parser.Structures.Records.Builder;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.General;

namespace Core.MasterFile.Parser.Structures.Records
{
    /// <summary>
    /// LSCR are loading screens.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class LSCR : Record
    {
        public readonly string EditorID;
        
        /// <summary>
        /// STAT FormID
        /// </summary>
        public readonly uint StaticObjectFormID;
        
        public readonly float InitialScale;
        
        public readonly Int16Vector3 InitialRotation;
        
        public readonly short MinRotationOffset;
        
        public readonly short MaxRotationOffset;
        
        public readonly Float32Vector3 InitialTranslation;
        
        public LSCR(LSCRBuilder builder) : base(builder.BaseInfo)
        {
            EditorID = builder.EditorID;
            StaticObjectFormID = builder.StaticObjectFormID;
            InitialScale = builder.InitialScale;
            InitialRotation = builder.InitialRotation;
            MinRotationOffset = builder.MinRotationOffset;
            MaxRotationOffset = builder.MaxRotationOffset;
            InitialTranslation = builder.InitialTranslation;
        }
    }
    
    // ReSharper disable once InconsistentNaming
    public class LSCRBuilder : IRecordBuilder
    {
        public string EditorID;
        public uint StaticObjectFormID;
        public float InitialScale;
        public Int16Vector3 InitialRotation;
        public short MinRotationOffset;
        public short MaxRotationOffset;
        public Float32Vector3 InitialTranslation;
        
        public Record BaseInfo { get; set; }
        
        public Record Build()
        {
            return new LSCR(this);
        }
    }
}