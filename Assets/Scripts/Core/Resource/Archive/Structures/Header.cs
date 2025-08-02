using System.Collections.Generic;
using Core.Resource.Archive.Structures.Enums;

namespace Core.Resource.Archive.Structures
{
    public class Header
    {
        public readonly uint Version;
        public readonly IReadOnlyCollection<ArchiveFlag> ArchiveFlags;
        public readonly uint FolderCount;
        public readonly uint FileCount;
        public readonly uint TotalFolderNameLength;
        public readonly uint TotalFileNameLength;
        public readonly IReadOnlyCollection<FileTypeFlag> FileFlags;
        
        public Header(HeaderBuilder builder)
        {
            Version = builder.Version;
            ArchiveFlags = builder.ArchiveFlags;
            FolderCount = builder.FolderCount;
            FileCount = builder.FileCount;
            TotalFolderNameLength = builder.TotalFolderNameLength;
            TotalFileNameLength = builder.TotalFileNameLength;
            FileFlags = builder.FileFlags;
        }
    }
    
    public class HeaderBuilder
    {
        public uint Version;
        public List<ArchiveFlag> ArchiveFlags = new();
        public uint FolderCount;
        public uint FileCount;
        public uint TotalFolderNameLength;
        public uint TotalFileNameLength;
        public List<FileTypeFlag> FileFlags = new();
        
        public Header Build()
        {
            return new Header(this);
        }
    }
}