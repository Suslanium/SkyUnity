namespace Core.Resource.Archive.Structures
{
    public class FolderRecord
    {
        /// <summary>
        /// Hash of the folder name (eg: menus\chargen). Must be all lower case, and use backslash as directory delimiter(s).
        /// </summary>
        public readonly long Hash;
        
        /// <summary>
        /// Amount of files in this folder.
        /// </summary>
        public readonly uint FileCount;
        
        /// <summary>
        /// Offset to file records for this folder.
        /// </summary>
        public readonly uint Offset;
        
        public FolderRecord(FolderRecordBuilder builder)
        {
            Hash = builder.Hash;
            FileCount = builder.FileCount;
            Offset = builder.Offset;
        }
    }
    
    public class FolderRecordBuilder
    {
        public long Hash;
        public uint FileCount;
        public uint Offset;
        
        public FolderRecord Build()
        {
            return new FolderRecord(this);
        }
    }
}