using Unity.Mathematics;

namespace Core.MasterFile.Parser.Structures.Records.FieldStructures.General
{
    /// <summary>
    /// RGB values range from 0 to 1
    /// </summary>
    public struct Float32ColorRGB
    {
        public readonly float3 RGB;
        
        public Float32ColorRGB(float r, float g, float b)
        {
            RGB = new float3(r, g, b);
        }
    }
}