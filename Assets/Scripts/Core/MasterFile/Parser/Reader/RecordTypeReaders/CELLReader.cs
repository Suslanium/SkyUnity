using System.IO;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser.Reader.RecordTypeReaders
{
    // ReSharper disable once InconsistentNaming
    public class CELLReader : RecordTypeReader<CELLBuilder>
    {
        private const string RecordType = "CELL";
        private const string EditorIDField = "EDID";
        private const string LocalizedNameField = "FULL";
        private const string FlagsField = "DATA";
        private const string GridPositionField = "XCLC";
        private const string LightingField = "XCLL";
        private const string LightingTemplateFormIdField = "LTMP";
        private const string WaterHeightField = "XCLW";
        private const string LocationFormIdField = "XLCN";
        private const string WaterFormIdField = "XCWT";
        private const string WaterEnvironmentMapField = "XWEM";
        private const string AcousticSpaceFormIdField = "XCAS";
        private const string MusicTypeFormIdField = "XCMO";
        private const string ImageSpaceFormIdField = "XCIM";
        
        public override string GetRecordType()
        {
            return RecordType;
        }

        protected override void ReadField(
            MasterFileProperties properties, 
            BinaryReader fileReader, 
            FieldInfo fieldInfo, 
            CELLBuilder builder)
        {
            switch (fieldInfo.Type)
            {
                case EditorIDField:
                    builder.EditorID = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case LocalizedNameField:
                    builder.Name = fileReader.ReadLocalizedString(fieldInfo.Size, properties.IsLocalized);
                    break;
                case FlagsField:
                    builder.CellFlag = fieldInfo.Size == 1 ? fileReader.ReadByte() : fileReader.ReadUInt16();
                    break;
                case GridPositionField:
                    builder.XGridPosition = fileReader.ReadInt32();
                    builder.YGridPosition = fileReader.ReadInt32();
                    break;
                case LightingField:
                    builder.LightingInfo = fileReader.ReadLightingField(fieldInfo);
                    break;
                case LightingTemplateFormIdField:
                    builder.LightingTemplateFormId = fileReader.ReadFormId();
                    break;
                case WaterHeightField:
                    builder.NonOceanWaterHeight = fileReader.ReadFloat32();
                    break;
                case LocationFormIdField:
                    builder.LocationFormId = fileReader.ReadFormId();
                    break;
                case WaterFormIdField:
                    builder.WaterFormId = fileReader.ReadFormId();
                    break;
                case WaterEnvironmentMapField:
                    builder.WaterEnvironmentMap = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case AcousticSpaceFormIdField:
                    builder.AcousticSpaceFormId = fileReader.ReadFormId();
                    break;
                case MusicTypeFormIdField:
                    builder.MusicTypeFormId = fileReader.ReadFormId();
                    break;
                case ImageSpaceFormIdField:
                    builder.ImageSpaceFormId = fileReader.ReadFormId();
                    break;
                default:
                    fileReader.BaseStream.Seek(fieldInfo.Size, SeekOrigin.Current);
                    break;
            }
        }
    }
}