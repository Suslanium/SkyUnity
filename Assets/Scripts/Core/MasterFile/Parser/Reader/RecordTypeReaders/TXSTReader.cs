using System.IO;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser.Reader.RecordTypeReaders
{
    // ReSharper disable once InconsistentNaming
    public class TXSTReader : RecordTypeReader<TXSTBuilder>
    {
        private const string RecordType = "TXST";
        private const string EditorIDField = "EDID";
        private const string DiffuseMapField = "TX00";
        private const string NormalMapField = "TX01";
        private const string MaskMapField = "TX02";
        private const string GlowMapField = "TX03";
        private const string DetailMapField = "TX04";
        private const string EnvironmentMapField = "TX05";
        private const string MultiLayerMapField = "TX06";
        private const string SpecularMapField = "TX07";
        private const string FlagsField = "DNAM";
        
        public override string GetRecordType()
        {
            return RecordType;
        }

        protected override void ReadField(
            MasterFileProperties properties,
            BinaryReader fileReader,
            FieldInfo fieldInfo,
            TXSTBuilder builder)
        {
            switch (fieldInfo.Type)
            {
                case EditorIDField:
                    builder.EditorID = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case DiffuseMapField:
                    builder.DiffuseMapPath = fileReader.ReadZString(fieldInfo.Size);
                    break; 
                case NormalMapField:
                    builder.NormalMapPath = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case MaskMapField:
                    builder.MaskMapPath = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case GlowMapField:
                    builder.GlowMapPath = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case DetailMapField:
                    builder.DetailMapPath = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case EnvironmentMapField:
                    builder.EnvironmentMapPath = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case MultiLayerMapField:
                    builder.MultiLayerMapPath = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case SpecularMapField:
                    builder.SpecularMapPath = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case FlagsField:
                    builder.Flags = fileReader.ReadUInt16();
                    break;
                default:
                    fileReader.BaseStream.Seek(fieldInfo.Size, SeekOrigin.Current);
                    break;
            }
        }
    }
}