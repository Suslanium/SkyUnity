using System.Collections.Generic;

namespace Core.Resource.Archive.Structures
{
    /// <summary>
    /// Block of file records for one folder
    /// </summary>
    public class FileRecordBlock
    {
        /// <summary>
        /// Name of the folder. Only present if Bit 1 (IncludeDirNames) of archiveFlags is set.
        /// </summary>
        public readonly string FolderName;

        /// <summary>
        /// File hash to file record map
        /// </summary>
        public readonly IReadOnlyDictionary<long, FileRecord> Files;
        
        public FileRecordBlock(FileRecordBlockBuilder builder)
        {
            FolderName = builder.FolderName;
            Files = builder.Files;
        }
    }
    
    public class FileRecordBlockBuilder
    {
        public string FolderName;
        public Dictionary<long, FileRecord> Files = new();
        
        public FileRecordBlock Build()
        {
            return new FileRecordBlock(this);
        }
    }
}