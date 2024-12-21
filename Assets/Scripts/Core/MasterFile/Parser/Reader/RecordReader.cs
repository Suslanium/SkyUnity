using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common;
using Core.MasterFile.Common.Structures;
using Core.MasterFile.Parser.Structures;
using Ionic.Zlib;

namespace Core.MasterFile.Parser.Reader
{
    public class RecordReader
    {
        private const uint DataIsCompressed = 0x00040000;
        private readonly ILogger _logger;
        private readonly IReadOnlyDictionary<string, IRecordTypeReader> _recordTypeReaders;

        public RecordReader(ILogger logger, IReadOnlyCollection<IRecordTypeReader> recordTypeReaders)
        {
            _logger = logger;
            _recordTypeReaders = recordTypeReaders.ToDictionary(reader => reader.GetRecordType());
        }

        /// <summary>
        /// Warning: stream position should be after the record type.
        /// </summary>
        public static Record ReadHeaderAndSkip(
            MasterFileProperties properties, 
            string recordType, 
            BinaryReader fileReader, 
            long streamPosition)
        {
            var baseRecordInfo = ReadBasicInfo(properties, recordType, fileReader, streamPosition);
            var recordDataStart = fileReader.BaseStream.Position;
            fileReader.BaseStream.Seek(recordDataStart + baseRecordInfo.DataSize, SeekOrigin.Begin);
            return baseRecordInfo;
        }

        /// <summary>
        /// Warning: stream position should be after the record type.
        /// </summary>
        public Record Read(
            MasterFileProperties properties,
            string recordType,
            BinaryReader fileReader,
            long streamPosition)
        {
            Record result;
            var recordInfo = ReadBasicInfo(properties, recordType, fileReader, streamPosition);
            var recordDataStart = fileReader.BaseStream.Position;
            if (!_recordTypeReaders.TryGetValue(recordType, out var recordReader))
            {
                _logger.Log($"No reader found for record type {recordType}", Severity.Warning);
                fileReader.BaseStream.Seek(recordDataStart + recordInfo.DataSize, SeekOrigin.Begin);
                return recordInfo;
            }

            if (Utils.IsFlagSet(recordInfo.Flag, DataIsCompressed))
            {
                var decompressedData = DecompressRecordData(fileReader, recordInfo.DataSize, recordInfo.FormId);
                var decompressedDataStream = new MemoryStream(decompressedData, false);
                var decompressedDataReader = new BinaryReader(decompressedDataStream);
                var decompressedRecordInfo = new Record(
                    type: recordInfo.Type,
                    dataSize: (uint)decompressedData.Length,
                    flag: recordInfo.Flag,
                    formID: recordInfo.FormId,
                    timestamp: recordInfo.Timestamp,
                    versionControlInfo: recordInfo.VersionControlInfo,
                    internalRecordVersion: recordInfo.InternalRecordVersion,
                    unknownData: recordInfo.UnknownData
                );

                result = recordReader.ReadFields(properties, decompressedRecordInfo, decompressedDataReader,
                    decompressedDataReader.BaseStream.Position);

                decompressedDataReader.Close();
                decompressedDataStream.Close();
            }
            else
            {
                result = recordReader.ReadFields(properties, recordInfo, fileReader, fileReader.BaseStream.Position);
            }

            //Explicitly set the position to the start of the next entry to recover from potential errors
            fileReader.BaseStream.Seek(recordDataStart + recordInfo.DataSize, SeekOrigin.Begin);
            return result;
        }

        private static Record ReadBasicInfo(
            MasterFileProperties properties, 
            string recordType, 
            BinaryReader fileReader, 
            long streamPosition)
        {
            fileReader.BaseStream.Seek(streamPosition, SeekOrigin.Begin);
            return new Record(
                type: recordType,
                dataSize: fileReader.ReadUInt32(),
                flag: fileReader.ReadUInt32(),
                formID: fileReader.ReadFormId(properties),
                timestamp: fileReader.ReadUInt16(),
                versionControlInfo: fileReader.ReadUInt16(),
                internalRecordVersion: fileReader.ReadUInt16(),
                unknownData: fileReader.ReadUInt16()
            );
        }

        private byte[] DecompressRecordData(BinaryReader fileReader, uint compressedDataSize, uint formId)
        {
            var decompressedSize = fileReader.ReadUInt32();
            var compressedData = fileReader.ReadBytes(checked((int)compressedDataSize));
            var decompressedData = new byte[decompressedSize];
            using var compressedDataStream = new MemoryStream(compressedData, false);
            using var decompressStream = new ZlibStream(compressedDataStream, CompressionMode.Decompress);
            var readAmount = decompressStream.Read(decompressedData, 0, checked((int)decompressedSize));
            if (readAmount != decompressedSize)
            {
                _logger.Log(
                    $"Decompressed data size mismatch for formId {formId:X}: expected {decompressedSize}, got {readAmount}",
                    Severity.Error
                );
            }

            return decompressedData;
        }
    }
}