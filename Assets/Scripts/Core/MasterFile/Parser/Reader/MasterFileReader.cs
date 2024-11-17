using System.Collections.Generic;
using System.IO;
using Core.MasterFile.Parser.Structures;

namespace Core.MasterFile.Parser.Reader
{
    public class MasterFileReader
    {
        private const string GroupEntryType = "GRUP";
        private const int GroupHeaderSize = 24;
        private const int EntryTypeLength = 4;
        private readonly RecordReader _recordReader;

        public MasterFileReader(RecordReader recordReader)
        {
            _recordReader = recordReader;
        }

        /// <summary>
        /// Reads an entire entry from the file.
        /// WARNING: If the entry is a group, it will read the entire group, including all subgroups and records.
        /// </summary>
        public MasterFileEntry ReadEntry(
            MasterFileProperties properties,
            BinaryReader fileReader,
            long entryStreamPosition)
        {
            fileReader.BaseStream.Seek(entryStreamPosition, SeekOrigin.Begin);
            var entryType = new string(fileReader.ReadChars(EntryTypeLength));
            if (entryType.Equals(GroupEntryType))
            {
                return ReadGroup(properties, fileReader);
            }
            else
            {
                return _recordReader.Read(
                    properties: properties,
                    recordType: entryType,
                    fileReader: fileReader,
                    streamPosition: fileReader.BaseStream.Position);
            }
        }

        /// <summary>
        /// Reads the "header" of an entry.
        /// In the case of a group, only the beginning of the group is read (without any records or subgroups inside).
        /// In the case of a record, only the beginning of the record is read (without any specific fields inside).
        /// Important thing to mention is that the position of the file reader is set to the start of
        /// the next entry if the read entry is a record, otherwise it is set to the start of the group's contents.
        /// </summary>
        public MasterFileEntry ReadEntryHeader(BinaryReader fileReader, long entryStreamPosition)
        {
            fileReader.BaseStream.Seek(entryStreamPosition, SeekOrigin.Begin);
            var entryType = new string(fileReader.ReadChars(EntryTypeLength));
            if (entryType.Equals(GroupEntryType))
            {
                return ReadGroupHeader(fileReader);
            }
            else
            {
                return RecordReader.ReadHeaderAndSkip(
                    recordType: entryType,
                    fileReader: fileReader,
                    streamPosition: fileReader.BaseStream.Position);
            }
        }

        private static Group ReadGroupHeader(BinaryReader fileReader)
        {
            return new Group(
                size: fileReader.ReadUInt32(),
                label: fileReader.ReadBytes(4),
                groupType: fileReader.ReadInt32(),
                timestamp: fileReader.ReadUInt16(),
                versionControlInfo: fileReader.ReadUInt16(),
                unknownData: fileReader.ReadUInt32());
        }

        private Group ReadGroup(MasterFileProperties properties, BinaryReader fileReader)
        {
            var groupEntries = new List<MasterFileEntry>();
            var group = new Group(
                size: fileReader.ReadUInt32(),
                label: fileReader.ReadBytes(4),
                groupType: fileReader.ReadInt32(),
                timestamp: fileReader.ReadUInt16(),
                versionControlInfo: fileReader.ReadUInt16(),
                unknownData: fileReader.ReadUInt32(),
                groupData: groupEntries);
            var dataStartPosition = fileReader.BaseStream.Position;
            //The group size includes the header size, so we need to subtract it
            while (fileReader.BaseStream.Position < dataStartPosition + group.Size - GroupHeaderSize)
            {
                groupEntries.Add(ReadEntry(properties, fileReader, fileReader.BaseStream.Position));
            }

            //Explicitly set the position to the start of the next entry to recover from potential errors
            fileReader.BaseStream.Seek(dataStartPosition + group.Size - GroupHeaderSize, SeekOrigin.Begin);
            return group;
        }
    }
}