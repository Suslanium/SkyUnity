using System.IO;
using System.Linq;
using Core.Resource.Archive.Structures;
using Core.Resource.Archive.Structures.Enums;

namespace Core.Resource.Archive
{
    public static class BsaParsingUtils
    {
        public static Header ParseHeader(BinaryReader binaryReader)
        {
            var header = new HeaderBuilder();
            binaryReader.BaseStream.Seek(4, SeekOrigin.Current);
            header.Version = binaryReader.ReadUInt32();
            binaryReader.BaseStream.Seek(4, SeekOrigin.Current);
            var archiveFlags = binaryReader.ReadUInt32();
            if ((archiveFlags & 0x1) != 0)
            {
                header.ArchiveFlags.Add(ArchiveFlag.IncludeDirNames);
            }

            if ((archiveFlags & 0x2) != 0)
            {
                header.ArchiveFlags.Add(ArchiveFlag.IncludeFileNames);
            }

            if ((archiveFlags & 0x4) != 0)
            {
                header.ArchiveFlags.Add(ArchiveFlag.CompressedArchive);
            }

            if ((archiveFlags & 0x8) != 0)
            {
                header.ArchiveFlags.Add(ArchiveFlag.RetainDirNames);
            }

            if ((archiveFlags & 0x10) != 0)
            {
                header.ArchiveFlags.Add(ArchiveFlag.RetainFileNames);
            }

            if ((archiveFlags & 0x20) != 0)
            {
                header.ArchiveFlags.Add(ArchiveFlag.RetainFileOffsets);
            }

            if ((archiveFlags & 0x40) != 0)
            {
                header.ArchiveFlags.Add(ArchiveFlag.Xbox360Archive);
            }

            if ((archiveFlags & 0x80) != 0)
            {
                header.ArchiveFlags.Add(ArchiveFlag.RetainStringsDuringStartup);
            }

            if ((archiveFlags & 0x100) != 0)
            {
                header.ArchiveFlags.Add(ArchiveFlag.EmbedFileNames);
            }

            if ((archiveFlags & 0x200) != 0)
            {
                header.ArchiveFlags.Add(ArchiveFlag.UsesXMemCodec);
            }

            header.FolderCount = binaryReader.ReadUInt32();
            header.FileCount = binaryReader.ReadUInt32();
            header.TotalFolderNameLength = binaryReader.ReadUInt32();
            header.TotalFileNameLength = binaryReader.ReadUInt32();
            var fileFlags = binaryReader.ReadUInt16();
            if ((fileFlags & 0x1) != 0)
            {
                header.FileFlags.Add(FileTypeFlag.Meshes);
            }

            if ((fileFlags & 0x2) != 0)
            {
                header.FileFlags.Add(FileTypeFlag.Textures);
            }

            if ((fileFlags & 0x4) != 0)
            {
                header.FileFlags.Add(FileTypeFlag.Menus);
            }

            if ((fileFlags & 0x8) != 0)
            {
                header.FileFlags.Add(FileTypeFlag.Sounds);
            }

            if ((fileFlags & 0x10) != 0)
            {
                header.FileFlags.Add(FileTypeFlag.Voices);
            }

            if ((fileFlags & 0x20) != 0)
            {
                header.FileFlags.Add(FileTypeFlag.Shaders);
            }

            if ((fileFlags & 0x40) != 0)
            {
                header.FileFlags.Add(FileTypeFlag.Trees);
            }

            if ((fileFlags & 0x80) != 0)
            {
                header.FileFlags.Add(FileTypeFlag.Fonts);
            }

            if ((fileFlags & 0x100) != 0)
            {
                header.FileFlags.Add(FileTypeFlag.Miscellaneous);
            }

            binaryReader.BaseStream.Seek(2, SeekOrigin.Current);

            return header.Build();
        }

        public static FolderRecord ParseFolderRecord(BinaryReader binaryReader, Header header)
        {
            var record = new FolderRecordBuilder
            {
                Hash = binaryReader.ReadInt64(),
                FileCount = binaryReader.ReadUInt32()
            };
            if (header.Version == 0x69) binaryReader.BaseStream.Seek(4, SeekOrigin.Current);
            record.Offset = binaryReader.ReadUInt32()-header.TotalFileNameLength;
            if (header.Version == 0x69) binaryReader.BaseStream.Seek(4, SeekOrigin.Current);
            return record.Build();
        }
        
        public static FileRecordBlock ParseFileRecordBlock(BinaryReader binaryReader, FolderRecord folderRecord, Header header)
        {
            var fileRecordBlock = new FileRecordBlockBuilder();
            binaryReader.BaseStream.Seek(folderRecord.Offset, SeekOrigin.Begin);
            var nameLength = binaryReader.ReadByte();
            fileRecordBlock.FolderName = new string(binaryReader.ReadChars(nameLength));
            for (var i = 0; i < folderRecord.FileCount; i++)
            {
                var fileRecord = ParseFileRecord(binaryReader, header);
                fileRecordBlock.Files[fileRecord.Hash] = fileRecord;
            }

            return fileRecordBlock.Build();
        }

        private static FileRecord ParseFileRecord(BinaryReader binaryReader, Header header)
        {
            var record = new FileRecordBuilder
            {
                Hash = binaryReader.ReadInt64(),
                Size = binaryReader.ReadUInt32(),
                Offset = binaryReader.ReadUInt32()
            };
            if (header.ArchiveFlags.Contains(ArchiveFlag.CompressedArchive))
            {
                record.IsCompressed = (record.Size & 0x40000000) == 0;
            }
            else
            {
                record.IsCompressed = (record.Size & 0x40000000) != 0;
            }

            if (record.IsCompressed)
            {
                record.Size -= 4;
            }

            return record.Build();
        }
    }
}