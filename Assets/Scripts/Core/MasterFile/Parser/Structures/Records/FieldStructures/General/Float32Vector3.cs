using Unity.Mathematics;

namespace Core.MasterFile.Parser.Structures.Records.FieldStructures.General
{
    public struct Float32Vector3
    {
        public readonly float3 XYZ;

        public Float32Vector3(float x, float y, float z)
        {
            XYZ = new float3(x, y, z);
        }
    }
}