using System.IO;
using Core.MasterFile.Common.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser.Reader.RecordTypeReaders
{
    // ReSharper disable once InconsistentNaming
    public class FURNReader : RecordTypeReader<FURNBuilder>
    {
        private const string RecordType = "FURN";
        private const string EditorIdField = "EDID";
        private const string InGameNameField = "FULL";
        private const string WorkbenchInfoField = "WBDT";
        private const string InteractionKeywordField = "KNAM";

        public override string GetRecordType()
        {
            return RecordType;
        }

        protected override void ReadField(
            MasterFileProperties properties, 
            BinaryReader fileReader, 
            FieldInfo fieldInfo, 
            FURNBuilder builder)
        {
            if (fileReader.TryReadModelField(builder.ModelInfo, properties, fieldInfo)) return;

            switch (fieldInfo.Type)
            {
                case EditorIdField:
                    builder.EditorID = fileReader.ReadZString(fieldInfo.Size);
                    break;
                case InGameNameField:
                    builder.InGameName = fileReader.ReadLocalizedString(fieldInfo.Size, properties);
                    break;
                case WorkbenchInfoField:
                    builder.WorkbenchType = fileReader.ReadByte();
                    builder.WorkbenchSkill = fileReader.ReadByte();
                    break;
                case InteractionKeywordField:
                    builder.InteractionKeyword = fileReader.ReadFormId(properties);
                    break;
            }
        }
    }
}