using System.IO;
using Core.MasterFile.Common.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser.Reader.RecordTypeReaders
{
    // ReSharper disable once InconsistentNaming
    public class LSCRReader : RecordTypeReader<LSCRBuilder>
    {
        private const string RecordType = "LSCR";
        private const string EditorIdField = "EDID";
        private const string StaticObjectFormIdField = "NNAM";
        private const string ScaleField = "SNAM";
        private const string RotationField = "RNAM";
        private const string RotationOffsetConstraintsField = "ONAM";
        private const string TranslationField = "XNAM";
        
        public override string GetRecordType()
        {
            return RecordType;
        }

        protected override void ReadField(MasterFileProperties properties, BinaryReader fileReader, FieldInfo fieldInfo, LSCRBuilder builder)
        {
            switch (fieldInfo.Type)
            {
                case EditorIdField:
                    builder.EditorID = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case StaticObjectFormIdField:
                    builder.StaticObjectFormID = fileReader.ReadFormId(properties);
                    break;
                case ScaleField:
                    builder.InitialScale = fileReader.ReadFloat32();
                    break;
                case RotationField:
                    builder.InitialRotation = fileReader.ReadInt16Vector3();
                    break;
                case RotationOffsetConstraintsField:
                    builder.MinRotationOffset = fileReader.ReadInt16();
                    builder.MaxRotationOffset = fileReader.ReadInt16();
                    break;
                case TranslationField:
                    builder.InitialTranslation = fileReader.ReadFloat32Vector3();
                    break;
            }
        }
    }
}