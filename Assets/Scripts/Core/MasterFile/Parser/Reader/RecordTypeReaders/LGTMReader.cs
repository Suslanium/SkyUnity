using System.IO;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser.Reader.RecordTypeReaders
{
    // ReSharper disable once InconsistentNaming
    public class LGTMReader : RecordTypeReader<LGTMBuilder>
    {
        private const string RecordType = "LGTM";
        private const string EditorIDField = "EDID";
        private const string LightingDataField = "DATA";
        
        public override string GetRecordType()
        {
            return RecordType;
        }

        protected override void ReadField(
            MasterFileProperties properties, 
            BinaryReader fileReader, 
            FieldInfo fieldInfo, 
            LGTMBuilder builder)
        {
            switch (fieldInfo.Type)
            {
                case EditorIDField:
                    builder.EditorID = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case LightingDataField:
                    builder.LightingInfo = fileReader.ReadLightingField(fieldInfo.Size);
                    break;
                default:
                    fileReader.BaseStream.Seek(fieldInfo.Size, SeekOrigin.Current);
                    break;
            }
        }
    }
}