using System.IO;
using Core.MasterFile.Common.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser.Reader.RecordTypeReaders
{
    // ReSharper disable once InconsistentNaming
    public class TREEReader : RecordTypeReader<TREEBuilder>
    {
        private const string RecordType = "TREE";
        private const string EditorIdField = "EDID";
        private const string DataField = "CNAM";
        
        public override string GetRecordType()
        {
            return RecordType;
        }

        protected override void ReadField(
            MasterFileProperties properties, 
            BinaryReader fileReader, 
            FieldInfo fieldInfo, 
            TREEBuilder builder)
        {
            if (fileReader.TryReadModelField(builder.ModelInfo, properties, fieldInfo)) return;

            switch (fieldInfo.Type)
            {
                case EditorIdField:
                    builder.EditorID = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case DataField:
                    builder.TrunkFlexibility = fileReader.ReadFloat32();
                    builder.BranchFlexibility = fileReader.ReadFloat32();
                    //Skipping 8 unknown floats
                    fileReader.BaseStream.Seek(32, SeekOrigin.Current);
                    builder.LeafAmplitude = fileReader.ReadFloat32();
                    builder.LeafFrequency = fileReader.ReadFloat32();
                    break;
            }
        }
    }
}