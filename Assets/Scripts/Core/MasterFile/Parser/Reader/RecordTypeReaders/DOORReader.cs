using System.IO;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser.Reader.RecordTypeReaders
{
    // ReSharper disable once InconsistentNaming
    public class DOORReader : RecordTypeReader<DOORBuilder>
    {
        private const string RecordType = "DOOR";
        private const string EditorIdField = "EDID";
        private const string ObjectBoundsField = "OBND";
        private const string OpenSoundField = "SNAM";
        private const string CloseSoundField = "ANAM";
        private const string LoopSoundField = "BNAM";
        private const string FlagsField = "FNAM";
        private const string RandomTeleportField = "TNAM";

        public override string GetRecordType()
        {
            return RecordType;
        }

        protected override void ReadField(
            MasterFileProperties properties, 
            BinaryReader fileReader, 
            FieldInfo fieldInfo, 
            DOORBuilder builder)
        {
            if (fileReader.TryReadModelField(builder.ModelInfo, fieldInfo)) return;
            
            switch (fieldInfo.Type)
            {
                case EditorIdField:
                    builder.EditorID = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case ObjectBoundsField:
                    builder.Bounds = fileReader.ReadObjectBounds();
                    break;
                case OpenSoundField:
                    builder.OpenSound = fileReader.ReadFormId();
                    break;
                case CloseSoundField:
                    builder.CloseSound = fileReader.ReadFormId();
                    break;
                case LoopSoundField:
                    builder.LoopSound = fileReader.ReadFormId();
                    break;
                case FlagsField:
                    builder.Flags = fileReader.ReadByte();
                    break;
                case RandomTeleportField:
                    builder.RandomTeleports.Add(fileReader.ReadFormId());
                    break;
                default:
                    fileReader.BaseStream.Seek(fieldInfo.Size, SeekOrigin.Current);
                    break;
            }
        }
    }
}