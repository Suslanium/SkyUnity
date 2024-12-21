using System.IO;
using Core.MasterFile.Common.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser.Reader.RecordTypeReaders
{
    // ReSharper disable once InconsistentNaming
    public class TES4Reader : RecordTypeReader<TES4Builder>
    {
        private const string RecordType = "TES4";
        private const string HeaderField = "HEDR";
        private const string AuthorField = "CNAM";
        private const string DescriptionField = "SNAM";
        private const string MasterFileField = "MAST";
        private const string OverridenFormsField = "ONAM";
        private const string NumberOfTagifiableStringsField = "INTV";
        
        public override string GetRecordType()
        {
            return RecordType;
        }

        protected override void ReadField(
            MasterFileProperties properties, 
            BinaryReader fileReader, 
            FieldInfo fieldInfo, 
            TES4Builder builder)
        {
            switch (fieldInfo.Type)
            {
                case HeaderField:
                    builder.Version = fileReader.ReadFloat32();
                    builder.EntryAmount = fileReader.ReadUInt32();
                    fileReader.ReadUInt32();
                    break;
                case AuthorField:
                    builder.Author = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case DescriptionField:
                    builder.Description = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case MasterFileField:
                    builder.MasterFiles.Add(fileReader.ReadZString(fieldInfo.Size));
                    break;
                case OverridenFormsField:
                    for (var i = 0; i < fieldInfo.Size / 4; i++)
                    {
                        builder.OverridenForms.Add(fileReader.ReadFormId(properties));
                    }
                    break;
                case NumberOfTagifiableStringsField:
                    builder.NumberOfTagifiableStrings = fileReader.ReadUInt32();
                    break;
            }
        }
    }
}