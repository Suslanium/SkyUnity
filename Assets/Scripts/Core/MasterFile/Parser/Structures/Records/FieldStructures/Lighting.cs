using Core.MasterFile.Parser.Structures.Records.FieldStructures.General;

namespace Core.MasterFile.Parser.Structures.Records.FieldStructures
{
    /// <summary>
    /// Cell lighting data
    /// </summary>
    public class Lighting
    {
        public readonly ColorRGBA AmbientColor;

        public readonly ColorRGBA DirectionalColor;

        public readonly int DirectionalRotationXY;

        public readonly int DirectionalRotationZ;

        public readonly float DirectionalFade;

        public readonly ColorRGBA FogNearColor;

        public readonly ColorRGBA FogFarColor;

        public readonly float FogNear;

        public readonly float FogFar;

        public readonly float FogMax;

        public readonly float LightFadeDistanceStart;

        public readonly float LightFadeDistanceEnd;

        /// <summary>
        /// Inherit flags - controls which parts are inherited from Lighting Template(only present in CELL records)
        /// <para>0x0001 - Ambient Color</para>
        /// <para>0x0002 - Directional Color</para>
        /// <para>0x0004 - Fog Color</para>
        /// <para>0x0008 - Fog Near</para>
        /// <para>0x0010 - Fog Far</para>
        /// <para>0x0020 - Directional Rot</para>
        /// <para>0x0040 - Directional Fade</para>
        /// <para>0x0080 - Clip Distance</para>
        /// <para>0x0100 - Fog Power</para>
        /// <para>0x0200 - Fog Max</para>
        /// <para>0x0400 - Light Fade Distances</para>
        /// </summary>
        public readonly uint InheritFlags;

        public Lighting(ColorRGBA ambientColor, ColorRGBA directionalColor, int directionalRotationXY,
            int directionalRotationZ, float directionalFade, ColorRGBA fogNearColor, ColorRGBA fogFarColor,
            float fogNear, float fogFar, float fogMax, float lightFadeDistanceStart, float lightFadeDistanceEnd,
            uint inheritFlags)
        {
            AmbientColor = ambientColor;
            DirectionalColor = directionalColor;
            DirectionalRotationXY = directionalRotationXY;
            DirectionalRotationZ = directionalRotationZ;
            DirectionalFade = directionalFade;
            FogNearColor = fogNearColor;
            FogFarColor = fogFarColor;
            FogNear = fogNear;
            FogFar = fogFar;
            FogMax = fogMax;
            LightFadeDistanceStart = lightFadeDistanceStart;
            LightFadeDistanceEnd = lightFadeDistanceEnd;
            InheritFlags = inheritFlags;
        }
    }
}