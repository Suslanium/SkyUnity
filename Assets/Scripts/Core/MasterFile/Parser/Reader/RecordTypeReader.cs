using System.IO;
using Core.MasterFile.Common.Structures;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records.Builder;

namespace Core.MasterFile.Parser.Reader
{
    public class FieldInfo
    {
        public readonly string Type;
        public readonly int Size;

        public FieldInfo(string type, int size)
        {
            Type = type;
            Size = size;
        }
    }

    public interface IRecordTypeReader
    {
        public Record ReadFields(
            MasterFileProperties properties,
            Record baseRecord,
            BinaryReader fileReader,
            long recordDataStartPosition);

        /// <summary>
        /// This method should return the type of record that this reader reads (e.g. "TES4", "MSTT", etc.).
        /// This is used to create a dictionary of readers for the RecordReader class.
        /// </summary>
        public string GetRecordType();
    }

    /// <summary>
    /// Base class for reading specific records
    /// </summary>
    public abstract class RecordTypeReader<TBuilder> : IRecordTypeReader where TBuilder : IRecordBuilder, new()
    {
        private const int FieldTypeLength = 4;
        private const string LongFieldSize = "XXXX";

        public Record ReadFields(
            MasterFileProperties properties,
            Record baseRecord,
            BinaryReader fileReader,
            long recordDataStartPosition)
        {
            var builder = new TBuilder
            {
                BaseInfo = baseRecord
            };
            fileReader.BaseStream.Seek(recordDataStartPosition, SeekOrigin.Begin);
            while (fileReader.BaseStream.Position < recordDataStartPosition + baseRecord.DataSize)
            {
                var fieldInfo = new FieldInfo(
                    type: new string(fileReader.ReadChars(FieldTypeLength)),
                    size: fileReader.ReadUInt16());

                //Due to field size being limited to a 16-bit integer,
                //XXXX fields are used to specify a 32-bit size of the next field
                if (fieldInfo.Type == LongFieldSize)
                {
                    var size = fileReader.ReadUInt32();
                    fieldInfo = new FieldInfo(
                        type: new string(fileReader.ReadChars(FieldTypeLength)),
                        size: checked((int)size)
                    );
                    //Skip field size since it was specified by the preceding XXXX field
                    fileReader.BaseStream.Seek(2, SeekOrigin.Current);
                }

                var fieldDataStartPosition = fileReader.BaseStream.Position;
                ReadField(properties, fileReader, fieldInfo, builder);
                //Fail-safe in case the field was not read correctly
                fileReader.BaseStream.Seek(fieldDataStartPosition + fieldInfo.Size, SeekOrigin.Begin);
            }

            return builder.Build();
        }

        public abstract string GetRecordType();

        /// <summary>
        /// This method should move (by reading, skipping, etc.) the file reader stream position
        /// EXACTLY by the size of the field. Not more, not less.
        /// </summary>
        protected abstract void ReadField(
            MasterFileProperties properties,
            BinaryReader fileReader,
            FieldInfo fieldInfo,
            TBuilder builder);
    }
}