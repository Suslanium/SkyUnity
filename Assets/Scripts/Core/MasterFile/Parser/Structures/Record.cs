﻿namespace Core.MasterFile.Parser.Structures
{
    /// <summary>
    /// Records generally correspond to objects (e.g., a creature, a game setting, a dialog entry),
    /// with the fine details of the object (e.g., health of a creature, a dialog entry test)
    /// being handled by the fields of the record. The fields of the records are different
    /// depending on the record type, so this superclass contains only common stuff.
    /// </summary>
    public class Record : MasterFileEntry
    {
        /// <summary>
        /// Type
        /// </summary>
        public readonly string Type;

        /// <summary>
        /// Size of data field
        /// </summary>
        public readonly uint DataSize;

        /// <summary>
        /// <para>Flags</para>
        /// <para>Flag -> Meaning</para>
        /// <para>0x00000001 -> (TES4) Master (ESM) file</para>
        /// <para>0x00000010 -> Deleted Group (bugged, see Groups)</para>
        /// <para>0x00000020 -> Deleted Record</para>
        /// <para>0x00000040 -> (GLOB) Constant;
        ///             (REFR) Hidden From Local Map (Needs Confirmation: Related to shields)</para>
        /// <para>0x00000080 -> (TES4) Localized - this will make Skyrim load the .STRINGS, .DLSTRINGS, and .ILSTRINGS files associated with the mod. If this flag is not set, lstrings are treated as zstrings.</para>
        /// <para>0x00000100 -> Must Update Anims;
        ///             (REFR) Inaccessible</para>
        /// <para>0x00000200 -> (TES4) Light Master (ESL) File. Data File;
        ///             (REFR) Hidden from local map;
        ///             (ACHR)   Starts dead;
        ///             (REFR) MotionBlurCastsShadows</para>
        /// <para>0x00000400 -> Quest item;
        ///             Persistent reference;
        ///             (LSCR) Displays in Main Menu</para>
        /// <para>0x00000800 -> Initially disabled</para>
        /// <para>0x00001000 -> Ignored</para>
        /// <para>0x00008000 -> Visible when distant</para>
        /// <para>0x00010000 -> (ACTI) Random Animation Start</para>
        /// <para>0x00020000 -> (ACTI) Dangerous;
        ///             Off limits (Interior cell);
        ///             Dangerous Can't be set without Ignore Object Interaction</para>
        /// <para>0x00040000 -> Data is compressed</para>
        /// <para>0x00080000 -> Can't wait</para>
        /// <para>0x00100000 -> (ACTI) Ignore Object Interaction;
        ///             Ignore Object Interaction Sets Dangerous Automatically</para>
        /// <para>0x00800000 -> Is Marker</para>
        /// <para>0x02000000 -> (ACTI) Obstacle;
        ///             (REFR) No AI Acquire</para>
        /// <para>0x04000000 -> NavMesh Gen - Filter</para>
        /// <para>0x08000000 -> NavMesh Gen - Bounding Box</para>
        /// <para>0x10000000 -> (FURN) Must Exit to Talk;
        ///             (REFR) Reflected By Auto Water</para>
        /// <para>0x20000000 -> (FURN/IDLM) Child Can Use;
        ///             (REFR) Don't Havok Settle</para>
        /// <para>0x40000000 -> NavMesh Gen - Ground;
        /// (REFR) NoRespawn</para>
        /// <para>0x80000000 -> (REFR) MultiBound</para>
        /// </summary>
        public readonly uint Flag;

        /// <summary>
        /// Record (form) identifier
        /// </summary>
        public readonly uint FormId;

        /// <summary>
        /// <para>Timestamp</para>
        /// <para>Skyrim: The low byte is the day of the month and the high byte is a combined value representing the month number and last digit of the year times 12. That value is offset, however, so the range is nominally 13-132, representing dates from January 20x4 through December 20x3. Lower values can be seen in Skyrim.esm, likely corresponding to older records held over from Oblivion where values of 1-12 represented 2003 (see the Oblivion version of this page for specifics).</para>
        /// <para>To derive the correct values, use the following formulae, where Y is the single-digit year, M is the month number, and HB is the high byte of the value:</para>
        /// <para>Y = ((HB - 1) / 12 + 3) MOD 10</para>
        /// <para>M = ((HB - 1) MOD 12) + 1</para>
        /// <para>HB = (((Y - 4) MOD 10) + 1) * 12 + M</para>
        /// <para>Skyrim SE: Bits are used to represent each part, with a two-digit year: 0bYYYYYYYMMMMDDDDD. Thus, January 25, 2021 would be (spaces added for clarity): 0b 0010101 0001 11001 or 0x2A39.</para>
        /// </summary>
        public readonly ushort Timestamp;

        /// <summary>
        /// <para>Version Control Info</para>
        /// <para>The low byte is the user id that last had the form checked out.</para>
        /// <para>The high byte is the user id (if any) that currently has the form checked out.</para>
        /// </summary>
        public readonly ushort VersionControlInfo;

        /// <summary>
        /// The internal version of the record. This can be used to distinguish certain records that have different field layouts or sizes.
        /// </summary>
        public readonly ushort InternalRecordVersion;

        /// <summary>
        /// Unknown. Values range between 0-15.
        /// </summary>
        public readonly ushort UnknownData;

        public Record(string type, uint dataSize, uint flag, uint formID, ushort timestamp, ushort versionControlInfo,
            ushort internalRecordVersion, ushort unknownData)
        {
            Type = type;
            DataSize = dataSize;
            Flag = flag;
            FormId = formID;
            Timestamp = timestamp;
            VersionControlInfo = versionControlInfo;
            InternalRecordVersion = internalRecordVersion;
            UnknownData = unknownData;
        }

        protected Record(Record baseInfo)
        {
            Type = baseInfo.Type;
            DataSize = baseInfo.DataSize;
            Flag = baseInfo.Flag;
            FormId = baseInfo.FormId;
            Timestamp = baseInfo.Timestamp;
            VersionControlInfo = baseInfo.VersionControlInfo;
            InternalRecordVersion = baseInfo.InternalRecordVersion;
            UnknownData = baseInfo.UnknownData;
        }
    }
}