namespace Core.Resource.Archive.Structures
{
    public class FileRecord
    {
        /// <summary>
        /// Hash of the file name (eg: race_sex_menu.xml). Must be all lower case.
        /// </summary>
        public readonly long Hash;

        /// <summary>
        /// Size of the file data.
        /// </summary>
        public readonly uint Size;

        public readonly bool IsCompressed;

        /// <summary>
        /// Offset to raw file data for this folder. Note that an "offset" is offset from file byte zero (start).
        /// </summary>
        public readonly uint Offset;

        public FileRecord(FileRecordBuilder builder)
        {
            Hash = builder.Hash;
            Size = builder.Size;
            IsCompressed = builder.IsCompressed;
            Offset = builder.Offset;
        }
    }

    public class FileRecordBuilder
    {
        public long Hash;
        public uint Size;
        public bool IsCompressed;
        public uint Offset;
        
        public FileRecord Build()
        {
            return new FileRecord(this);
        }
    }
}