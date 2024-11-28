using System.IO;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser.Reader.RecordTypeReaders
{
    // ReSharper disable once InconsistentNaming
    public class LIGHReader : RecordTypeReader<LIGHBuilder>
    {
        private const string RecordType = "LIGH";
        private const string EditorIDField = "EDID";
        private const string InventoryIconField = "ICON";
        private const string MessageIconField = "MICO";
        private const string LightingDataField = "DATA";
        private const string FadeValueField = "FNAM";
        private const string HoldingSoundField = "SNAM";
        
        public override string GetRecordType()
        {
            return RecordType;
        }

        protected override void ReadField(
            MasterFileProperties properties,
            BinaryReader fileReader,
            FieldInfo fieldInfo,
            LIGHBuilder builder)
        {
            if (fileReader.TryReadModelField(builder.ModelInfo, fieldInfo)) return;

            switch (fieldInfo.Type)
            {
                case EditorIDField:
                    builder.EditorID = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case InventoryIconField:
                    builder.InventoryIconFilename = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case MessageIconField:
                    builder.MessageIconFilename = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case LightingDataField:
                    builder.Time = fileReader.ReadInt32();
                    builder.Radius = fileReader.ReadUInt32();
                    builder.Color = fileReader.ReadByteColorRGBA();
                    builder.Flags = fileReader.ReadUInt32();
                    builder.FalloffExponent = fileReader.ReadFloat32();
                    builder.Fov = fileReader.ReadFloat32();
                    builder.NearClip = fileReader.ReadFloat32();
                    builder.InversePeriod = fileReader.ReadFloat32();
                    builder.IntensityAmplitude = fileReader.ReadFloat32();
                    builder.MovementAmplitude = fileReader.ReadFloat32();
                    builder.Value = fileReader.ReadUInt32();
                    builder.Weight = fileReader.ReadFloat32();
                    break;
                case FadeValueField:
                    builder.Fade = fileReader.ReadFloat32();
                    break;
                case HoldingSoundField:
                    builder.HoldingSoundFormID = fileReader.ReadFormId();
                    break;
                default:
                    fileReader.BaseStream.Seek(fieldInfo.Size, SeekOrigin.Current);
                    break;
            }
        }
    }
}