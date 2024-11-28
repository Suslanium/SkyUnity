using System.IO;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser.Reader.RecordTypeReaders
{
    // ReSharper disable once InconsistentNaming
    public class MSTTReader : RecordTypeReader<MSTTBuilder>
    {
        private const string RecordType = "MSTT";
        private const string EditorIdField = "EDID";
        private const string AmbientLoopingSoundFormIdField = "SNAM";
        
        public override string GetRecordType()
        {
            return RecordType;
        }

        protected override void ReadField(
            MasterFileProperties properties, 
            BinaryReader fileReader, 
            FieldInfo fieldInfo, 
            MSTTBuilder builder)
        {
            if (fileReader.TryReadModelField(builder.ModelInfo, fieldInfo)) return;
            
            switch (fieldInfo.Type)
            {
                case EditorIdField:
                    builder.EditorID = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case AmbientLoopingSoundFormIdField:
                    builder.AmbientLoopingSoundFormID = fileReader.ReadFormId();
                    break;
                default:
                    fileReader.BaseStream.Seek(fieldInfo.Size, SeekOrigin.Current);
                    break;
            }
        }
    }
}