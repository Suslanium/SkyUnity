using System.IO;
using Core.MasterFile.Common.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser.Reader.RecordTypeReaders
{
    // ReSharper disable once InconsistentNaming
    public class STATReader : RecordTypeReader<STATBuilder>
    {
        private const string RecordType = "STAT";
        private const string EditorIdField = "EDID";
        private const string DataField = "DATA";
        
        public override string GetRecordType()
        {
            return RecordType;
        }

        protected override void ReadField(
            MasterFileProperties properties, 
            BinaryReader fileReader, 
            FieldInfo fieldInfo, 
            STATBuilder builder)
        {
            if (fileReader.TryReadModelField(builder.ModelInfo, properties, fieldInfo)) return;

            switch (fieldInfo.Type)
            {
                case EditorIdField:
                    builder.EditorID = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case DataField:
                    builder.DirMaterialMaxAngle = fileReader.ReadFloat32();
                    builder.DirectionalMaterialFormID = fileReader.ReadFormId(properties);
                    break;
            }
        }
    }
}