using System.Collections.Generic;
using Core.MasterFile.Parser.Structures.Records.Builder;

namespace Core.MasterFile.Parser.Structures.Records
{
    /// <summary>
    /// TES4 is the header record for the mod file. It contains info like author, description, file type, and masters list.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TES4 : Record
    {
        /// <summary>
        /// File version (0.94 for older files; 1.7 for more recent ones).
        /// </summary>
        public readonly float Version;

        /// <summary>
        /// Number of records and groups (not including TES4 record itself).
        /// </summary>
        public readonly uint EntryAmount;

        public readonly string Author;

        public readonly string Description;

        /// <summary>
        /// A list of required master files for this 
        /// </summary>
        public readonly IReadOnlyList<string> MasterFiles;

        /// <summary>
        /// <para>Overridden forms</para>
        /// <para>This record only appears in ESM flagged files which override their masters' cell children.</para>
        /// <para>An ONAM subrecord will list, exclusively, FormIDs of overridden cell children (ACHR, LAND, NAVM, PGRE, PHZD, REFR).</para>
        /// <para>Number of records is based solely on field size.</para>
        /// </summary>
        public readonly IReadOnlyList<uint> OverridenForms;

        /// <summary>
        /// Number of strings that can be tagified (used only for TagifyMasterfile command-line option of the CK).
        /// </summary>
        public readonly uint NumberOfTagifiableStrings;

        /// <summary>
        /// Some kind of counter. Appears to be related to masters.
        /// </summary>
        public readonly uint Incc;
        
        public TES4(TES4Builder builder) : base(builder.BaseInfo)
        {
            Version = builder.Version;
            EntryAmount = builder.EntryAmount;
            Author = builder.Author;
            Description = builder.Description;
            MasterFiles = builder.MasterFiles;
            OverridenForms = builder.OverridenForms;
            NumberOfTagifiableStrings = builder.NumberOfTagifiableStrings;
            Incc = builder.Incc;
        }
    }
    
    // ReSharper disable once InconsistentNaming
    public class TES4Builder : IRecordBuilder
    {
        public float Version;
        public uint EntryAmount;
        public string Author;
        public string Description;
        public List<string> MasterFiles = new();
        public List<uint> OverridenForms = new();
        public uint NumberOfTagifiableStrings;
        public uint Incc;
        
        public Record BaseInfo { get; set; }
        
        public Record Build()
        {
            return new TES4(this);
        }
    }
}