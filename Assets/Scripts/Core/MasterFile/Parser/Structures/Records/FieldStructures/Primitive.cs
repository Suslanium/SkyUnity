using Core.MasterFile.Parser.Structures.Records.FieldStructures.General;

namespace Core.MasterFile.Parser.Structures.Records.FieldStructures
{
    /// <summary>
    /// A field that describes a primitive object, such as a box or sphere.
    /// Currently, it is mostly used for occlusion boxes.
    /// </summary>
    public class Primitive
    {
        public readonly Float32Vector3 Bounds;
        
        public readonly Float32ColorRGB Color;

        /// <summary>
        /// float - unknown: 0.15, 0.2, 0.25, 1.0 seen; same for any given base object
        /// </summary>
        public readonly float Unknown;
        
        /// <summary>
        /// uint32 - unknown: 1-4 seen
        /// 1 - Box
        /// 2 - Sphere
        /// 3 - Portal Box
        /// 4 - Unknown
        /// </summary>
        public readonly uint Unknown2;

        public Primitive(Float32Vector3 bounds, Float32ColorRGB color, float unknown, uint unknown2)
        {
            Bounds = bounds;
            Color = color;
            Unknown = unknown;
            Unknown2 = unknown2;
        }
    }
}