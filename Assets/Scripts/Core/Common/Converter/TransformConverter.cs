using Unity.Burst;
using Unity.Mathematics;

namespace Core.Common.Converter
{
    [BurstCompile]
    public static class TransformConverter
    {
        private const float MeterInSkyrimUnits = Constants.MeterInSkyrimUnits;
        private static readonly float3 Right = new float3(1, 0, 0);
        private static readonly float3 Up = new float3(0, 1, 0);
        private static readonly float3 Forward = new float3(0, 0, 1);
        
        [BurstCompile]
        public static void SwapYZ(in float3 vector, out float3 result)
        {
            result = new float3(vector.x, vector.z, vector.y);
        }
        
        [BurstCompile]
        public static void SkyrimPointToUnityPoint(in float3 vector, out float3 result)
        {
            SwapYZ(vector, out var swapped);
            result = swapped * MeterInSkyrimUnits;
        }

        [BurstCompile]
        public static void SkyrimRadiansToUnityQuaternion(in float3 skyrimRadians, out quaternion result)
        {
            SwapYZ(skyrimRadians, out var radians);
            
            var xRot = quaternion.AxisAngle(Right, radians.x);
            var yRot = quaternion.AxisAngle(Up, radians.y);
            var zRot = quaternion.AxisAngle(Forward, radians.z);
            
            result = math.mul(math.mul(zRot, yRot), xRot);
        }
    }
}