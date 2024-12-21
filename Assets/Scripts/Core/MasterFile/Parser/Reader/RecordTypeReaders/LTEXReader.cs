using System.IO;
using Core.MasterFile.Common.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser.Reader.RecordTypeReaders
{
    // ReSharper disable once InconsistentNaming
    public class LTEXReader : RecordTypeReader<LTEXBuilder>
    {
        private const string RecordType = "LTEX";
        private const string EditorIdField = "EDID";
        private const string TextureFormIdField = "TNAM";
        
        public override string GetRecordType()
        {
            return RecordType;
        }

        protected override void ReadField(
            MasterFileProperties properties, 
            BinaryReader fileReader, 
            FieldInfo fieldInfo, 
            LTEXBuilder builder)
        {
            switch (fieldInfo.Type)
            {
                case EditorIdField:
                    builder.EditorID = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case TextureFormIdField:
                    builder.TextureFormID = fileReader.ReadFormId(properties);
                    break;
            }
        }
    }
}