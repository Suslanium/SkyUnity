using Core.MasterFile.Parser.Structures.Records.Builder;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.General;

namespace Core.MasterFile.Parser.Structures.Records
{
    /// <summary>
    /// <para>WRLD records contain the data for a 'world', which can be all of Tamriel or a handful of cells.</para>
    /// In the WRLD GRUP, following each WRLD record is a nested GRUP which contains a single CELL record (presumably the starting location for the world) and a number of Exterior Cell Block groups (see the group table). Each Exterior Cell Block group contains Exterior Cell Sub-Block groups, each of which contain CELL records.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class WRLD : Record
    {
        /// <summary>
        /// The name of this worldspace used in the construction kit
        /// </summary>
        public readonly string EditorID;

        /// <summary>
        /// The name of this worldspace used in the game(lstring)
        /// </summary>
        public readonly LocalizedString InGameName;

        public readonly short CenterCellGridX;
        
        public readonly short CenterCellGridY;

        /// <summary>
        /// Interior Lighting LGTM
        /// </summary>
        public readonly uint InteriorLightingFormId;

        /// <summary>
        /// <para>Flags</para>
        /// <para>0x01 - Small World</para>
        /// <para>0x02 - Can't Fast Travel From Here</para>
        /// <para>0x04</para>
        /// <para>0x08 - No LOD Water</para>
        /// <para>0x10 - No Landscape</para>
        /// <para>0x20 - No Sky</para>
        /// <para>0x40 - Fixed Dimensions</para>
        /// <para>0x80 - No Grass</para>
        /// </summary>
        public readonly byte WorldFlag;

        /// <summary>
        /// Form ID of the parent worldspace.
        /// </summary>
        public readonly uint ParentWorldFormId;
        
        /// <summary>
        /// <para>Use flags - Set if parts are inherited from parent worldspace WNAM</para>
        /// <para>0x01 - Use Land Data (DNAM)</para>
        /// <para>0x02 - Use LOD Data (NAM3, NAM4)</para>
        /// <para>0x04 - Use Map Data (MNAM, MODL)</para>
        /// <para>0x08 - Use Water Data (NAM2)</para>
        /// <para>0x10 - unknown</para>
        /// <para>0x20 - Use Climate Data (CNAM)</para>
        /// <para>0x40 - Use Sky Cell</para>
        /// </summary>
        public readonly ushort ParentWorldRelatedFlags;

        /// <summary>
        /// Location LCTN
        /// </summary>
        public readonly uint ExitLocationFormId;

        /// <summary>
        /// CLMT reference
        /// </summary>
        public readonly uint ClimateFormId;

        /// <summary>
        /// -27000 for Tamriel
        /// </summary>
        public readonly float LandLevel;
        
        /// <summary>
        /// -14000.0 for Tamriel
        /// </summary>
        public readonly float OceanWaterLevel;
        
        public WRLD(WRLDBuilder builder) : base(builder.BaseInfo)
        {
            EditorID = builder.EditorID;
            InGameName = builder.InGameName;
            CenterCellGridX = builder.CenterCellGridX;
            CenterCellGridY = builder.CenterCellGridY;
            InteriorLightingFormId = builder.InteriorLightingFormId;
            WorldFlag = builder.WorldFlag;
            ParentWorldFormId = builder.ParentWorldFormId;
            ParentWorldRelatedFlags = builder.ParentWorldRelatedFlags;
            ExitLocationFormId = builder.ExitLocationFormId;
            ClimateFormId = builder.ClimateFormId;
            LandLevel = builder.LandLevel;
            OceanWaterLevel = builder.OceanWaterLevel;
        }
    }
    
    // ReSharper disable once InconsistentNaming
    public class WRLDBuilder : IRecordBuilder
    {
        public string EditorID;
        public LocalizedString InGameName;
        public short CenterCellGridX;
        public short CenterCellGridY;
        public uint InteriorLightingFormId;
        public byte WorldFlag;
        public uint ParentWorldFormId;
        public ushort ParentWorldRelatedFlags;
        public uint ExitLocationFormId;
        public uint ClimateFormId;
        public float LandLevel;
        public float OceanWaterLevel;
        
        public Record BaseInfo { get; set; }
        
        public Record Build()
        {
            return new WRLD(this);
        }
    }
}