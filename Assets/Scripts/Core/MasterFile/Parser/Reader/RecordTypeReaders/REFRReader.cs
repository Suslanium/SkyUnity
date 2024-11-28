using System.IO;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;
using Core.MasterFile.Parser.Structures.Records.FieldStructures;

namespace Core.MasterFile.Parser.Reader.RecordTypeReaders
{
    // ReSharper disable once InconsistentNaming
    public class REFRReader : RecordTypeReader<REFRBuilder>
    {
        private const string RecordType = "REFR";
        private const string EditorIdField = "EDID";
        private const string ReferenceFormIdField = "NAME";
        private const string PrimitiveField = "XPRM";
        private const string LightingTemplateFormIdField = "LNAM";
        private const string ImageSpaceFormIdField = "INAM";
        private const string EmittedLightFormIdField = "XEMI";
        private const string DoorTeleportInfoFormIdField = "XTEL";
        private const string LeveledItemBaseFormIdField = "XLIB";
        private const string LocationFormIdField = "XLRT";
        private const string LocationalDataField = "DATA";
        private const string ScaleField = "XSCL";
        private const string RadiusField = "XRDS";
        private const string LightDataField = "XLIG";

        //Occlusion culling-related fields
        private const string PortalDestinationsField = "XPOD";
        private const string LinkedRoomFormIdsField = "XLRM";

        public override string GetRecordType()
        {
            return RecordType;
        }

        protected override void ReadField(
            MasterFileProperties properties,
            BinaryReader fileReader,
            FieldInfo fieldInfo,
            REFRBuilder builder)
        {
            switch (fieldInfo.Type)
            {
                case EditorIdField:
                    builder.EditorID = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case ReferenceFormIdField:
                    builder.BaseObjectFormId = fileReader.ReadFormId();
                    break;
                case PrimitiveField:
                    var bounds = fileReader.ReadFloat32Vector3();
                    var color = fileReader.ReadFloat32ColorRGB();
                    var unknownFloat = fileReader.ReadFloat32();
                    var unknownInt = fileReader.ReadUInt32();
                    builder.Primitive = new Primitive(bounds, color, unknownFloat, unknownInt);
                    break;
                case PortalDestinationsField:
                    var originFormID = fileReader.ReadFormId();
                    var destinationFormID = fileReader.ReadFormId();
                    builder.PortalDestinations = new PortalInfo(
                        originFormID,
                        destinationFormID);
                    break;
                case LightingTemplateFormIdField:
                    builder.LightingTemplateFormId = fileReader.ReadFormId();
                    break;
                case ImageSpaceFormIdField:
                    builder.ImageSpaceFormId = fileReader.ReadFormId();
                    break;
                case LinkedRoomFormIdsField:
                    builder.LinkedRoomFormIds.Add(fileReader.ReadFormId());
                    break;
                case EmittedLightFormIdField:
                    builder.EmittedLightFormId = fileReader.ReadFormId();
                    break;
                case DoorTeleportInfoFormIdField:
                    builder.DoorTeleport = new DoorTeleport(
                        destinationDoorReference: fileReader.ReadFormId(),
                        destinationPosition: fileReader.ReadFloat32Vector3(),
                        destinationRotation: fileReader.ReadFloat32Vector3(),
                        flag: fileReader.ReadUInt32());
                    break;
                case LeveledItemBaseFormIdField:
                    builder.LeveledItemBaseFormId = fileReader.ReadFormId();
                    break;
                case LocationFormIdField:
                    builder.LocationReferenceFormId = fileReader.ReadFormId();
                    break;
                case LocationalDataField:
                    builder.Position = fileReader.ReadFloat32Vector3();
                    builder.Rotation = fileReader.ReadFloat32Vector3();
                    break;
                case ScaleField:
                    builder.Scale = fileReader.ReadFloat32();
                    break;
                case RadiusField:
                    builder.Radius = fileReader.ReadFloat32();
                    break;
                case LightDataField:
                    fileReader.BaseStream.Seek(4, SeekOrigin.Current);
                    builder.FadeOffset = fileReader.ReadFloat32();
                    fileReader.BaseStream.Seek(fieldInfo.Size == 16 ? 8 : 12, SeekOrigin.Current);
                    break;
                default:
                    fileReader.BaseStream.Seek(fieldInfo.Size, SeekOrigin.Current);
                    break;
            }
        }
    }
}