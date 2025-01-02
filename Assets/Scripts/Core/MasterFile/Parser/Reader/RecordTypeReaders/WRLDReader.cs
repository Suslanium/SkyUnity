using System.IO;
using Core.MasterFile.Common.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser.Reader.RecordTypeReaders
{
    // ReSharper disable once InconsistentNaming
    public class WRLDReader : RecordTypeReader<WRLDBuilder>
    {
        private const string RecordType = "WRLD";
        private const string EditorIdField = "EDID";
        private const string LocalizedNameField = "FULL";
        private const string CenterCellCoordinatesField = "WCTR";
        private const string InteriorLightingFormIdField = "LTMP";
        private const string FlagsField = "DATA";
        private const string ParentWorldSpaceFormIdField = "WNAM";
        private const string ExitLocationFormIdField = "XLCN";
        private const string ClimateFormIdField = "CNAM";
        private const string LandDataField = "DNAM";
        private const string ParentWorldSpaceFlagsField = "PNAM";
        
        public override string GetRecordType()
        {
            return RecordType;
        }

        protected override void ReadField(
            MasterFileProperties properties, 
            BinaryReader fileReader, 
            FieldInfo fieldInfo, 
            WRLDBuilder builder)
        {
            switch (fieldInfo.Type)
            {
                case EditorIdField:
                    builder.EditorID = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case LocalizedNameField:
                    builder.InGameName = fileReader.ReadLocalizedString(fieldInfo.Size, properties);
                    break;
                case CenterCellCoordinatesField:
                    builder.CenterCellGridX = fileReader.ReadInt16();
                    builder.CenterCellGridY = fileReader.ReadInt16();
                    break;
                case InteriorLightingFormIdField:
                    builder.InteriorLightingFormId = fileReader.ReadFormId(properties);
                    break;
                case FlagsField:
                    builder.WorldFlag = fileReader.ReadByte();
                    break;
                case ParentWorldSpaceFormIdField:
                    builder.ParentWorldFormId = fileReader.ReadFormId(properties);
                    break;
                case ExitLocationFormIdField:
                    builder.ExitLocationFormId = fileReader.ReadFormId(properties);
                    break;
                case ClimateFormIdField:
                    builder.ClimateFormId = fileReader.ReadFormId(properties);
                    break;
                case LandDataField:
                    builder.LandLevel = fileReader.ReadFloat32();
                    builder.OceanWaterLevel = fileReader.ReadFloat32();
                    break;
                case ParentWorldSpaceFlagsField:
                    builder.ParentWorldRelatedFlags = fileReader.ReadUInt16();
                    break;
            }
        }
    }
}