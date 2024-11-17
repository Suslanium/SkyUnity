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
        
        public Lighting(LightingBuilder builder)
        {
            AmbientColor = builder.AmbientColor;
            DirectionalColor = builder.DirectionalColor;
            DirectionalRotationXY = builder.DirectionalRotationXY;
            DirectionalRotationZ = builder.DirectionalRotationZ;
            DirectionalFade = builder.DirectionalFade;
            FogNearColor = builder.FogNearColor;
            FogFarColor = builder.FogFarColor;
            FogNear = builder.FogNear;
            FogFar = builder.FogFar;
            FogMax = builder.FogMax;
            LightFadeDistanceStart = builder.LightFadeDistanceStart;
            LightFadeDistanceEnd = builder.LightFadeDistanceEnd;
            InheritFlags = builder.InheritFlags;
        }
    }

    public class LightingBuilder
    {
        public ColorRGBA AmbientColor;
        public ColorRGBA DirectionalColor;
        public int DirectionalRotationXY;
        public int DirectionalRotationZ;
        public float DirectionalFade;
        public ColorRGBA FogNearColor;
        public ColorRGBA FogFarColor;
        public float FogNear;
        public float FogFar;
        public float FogMax;
        public float LightFadeDistanceStart;
        public float LightFadeDistanceEnd;
        public uint InheritFlags;
        
        public Lighting Build()
        {
            return new Lighting(this);
        }
    }
}