using Core.MasterFile.Parser.Structures.Records.FieldStructures;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.General;

namespace Core.MasterFile.Parser.Structures.Records
{
    /// <summary>
    /// <para>CELL records contain the data for either interior (CELL top group) or exterior (WRLD top group) cells. They follow the same format in both cases. The CELL record is followed by a group containing the references for the cell, organized into subgroups. Persistent references are defined in the 'persistent children' subgroup while temporary references are defined in the 'temporary children' subgroup. The 'persistent children' subgroup must occur before the 'temporary children' subgroup.</para>
    /// <para>The block and sub-block groups for an interior cell can be determined either by the label field of the group headers of the block and sub-block, or by the last two decimal digits of the Form ID. For example, for Form ID 0x138CA, which is 80,074 in decimal, 4 indicates that this is block 4, and the 7 indicates that it's sub-block 7.</para>
    /// <para>The block and sub-block groups for an exterior cell can be determined either by the label filed of the group headers of the block and sub-block, or by the X, Y coordinates of the cell from the XCLC field. Each block contains 16 sub-blocks (4x4) and each sub-block contains 64 cells (8x8). So, to get the sub-block number, divide the coordinates by 8 (rounding down), and the block number can be determined by dividing the sub-block numbers by 4 (again, rounding down). Thus, a cell with the X, Y coordinates 31, 50 would be in floor(31 / 8) = 3, floor(50 / 8) = 6, so sub-block 3, 6. Dividing again, only this time by 4, it would be in block 0, 1.</para>
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class CELL : Record
    {
        public readonly string EditorID;

        public readonly LocalizedString Name;

        /// <summary>
        /// <para>flags - Sometimes the field is only one byte long</para>
        /// <para>0x0001 - Interior</para>
        /// <para>0x0002 - Has Water</para>
        /// <para>0x0004 - not Can't Travel From Here - only valid for interior cells</para>
        /// <para>0x0008 - No LOD Water</para>
        /// <para>0x0020 - Public Area</para>
        /// <para>0x0040 - Hand Changed</para>
        /// <para>0x0080 - Show Sky</para>
        /// <para>0x0100 - Use Sky Lighting</para>
        /// </summary>
        public readonly ushort CellFlag;

        /// <summary>
        /// Always in exterior cells and never in interior cells.
        /// </summary>
        public readonly int XGridPosition;

        /// <summary>
        /// Always in exterior cells and never in interior cells.
        /// </summary>
        public readonly int YGridPosition;

        public readonly Lighting CellLightingInfo;

        /// <summary>
        /// The lighting template for this cell.
        /// </summary>
        public readonly uint LightingTemplateReference;

        /// <summary>
        /// <para>Non-ocean water-height in cell, is used for rivers, ponds etc., ocean-water is globally defined elsewhere.</para>
        /// <para>0x7F7FFFFF reserved as ID for "no water present", it is also the maximum positive float.</para>
        /// <para>0x4F7FFFC9 is a bug in the CK, this is the maximum unsigned integer 2^32-1 cast to a float and means the same as above</para>
        /// <para>0xCF000000 could be a bug as well, this is the maximum signed negative integer -2^31 cast to a float</para>
        /// </summary>
        public readonly float NonOceanWaterHeight;

        /// <summary>
        /// The location for (of?) this cell.
        /// </summary>
        public readonly uint LocationReference;

        /// <summary>
        /// The water for (of?) this cell.
        /// </summary>
        public readonly uint WaterReference;

        /// <summary>
        /// Water Environment Map (only interior cells)
        /// </summary>
        public readonly string WaterEnvironmentMap;

        /// <summary>
        /// The acoustic space for this cell.
        /// </summary>
        public readonly uint AcousticSpaceReference;

        /// <summary>
        /// The music type for this cell.
        /// </summary>
        public readonly uint MusicTypeReference;

        /// <summary>
        /// The image space for this cell.
        /// </summary>
        public readonly uint ImageSpaceReference;

        public CELL(Record baseInfo, string editorID, LocalizedString name, ushort cellFlag, int xGridPosition,
            int yGridPosition, Lighting cellLightingInfo, uint lightingTemplateReference, float nonOceanWaterHeight,
            uint locationReference, uint waterReference, string waterEnvironmentMap, uint acousticSpaceReference,
            uint musicTypeReference, uint imageSpaceReference) : base(baseInfo)
        {
            EditorID = editorID;
            Name = name;
            CellFlag = cellFlag;
            XGridPosition = xGridPosition;
            YGridPosition = yGridPosition;
            CellLightingInfo = cellLightingInfo;
            LightingTemplateReference = lightingTemplateReference;
            NonOceanWaterHeight = nonOceanWaterHeight;
            LocationReference = locationReference;
            WaterReference = waterReference;
            WaterEnvironmentMap = waterEnvironmentMap;
            AcousticSpaceReference = acousticSpaceReference;
            MusicTypeReference = musicTypeReference;
            ImageSpaceReference = imageSpaceReference;
        }
    }
}