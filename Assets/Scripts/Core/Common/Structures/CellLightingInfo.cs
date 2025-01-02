using Unity.Mathematics;
using UnityEngine;

namespace Core.Common.Structures
{
    public class CellLightingInfo
    {
        public readonly Color32 AmbientLightColor;
        public readonly bool IsDirectionalLightEnabled;
        public readonly Color32 DirectionalLightColor;
        public readonly quaternion DirectionalLightRotation;
        public readonly bool IsFogEnabled;
        public readonly Color32 FogColor;
        public readonly float FogStartDistance;
        public readonly float FogEndDistance;
        
        public CellLightingInfo(CellLightingInfoBuilder builder)
        {
            AmbientLightColor = builder.AmbientLightColor;
            IsDirectionalLightEnabled = builder.IsDirectionalLightEnabled;
            DirectionalLightColor = builder.DirectionalLightColor;
            DirectionalLightRotation = builder.DirectionalLightRotation;
            IsFogEnabled = builder.IsFogEnabled;
            FogColor = builder.FogColor;
            FogStartDistance = builder.FogStartDistance;
            FogEndDistance = builder.FogEndDistance;
        }
    }
    
    public class CellLightingInfoBuilder
    {
        public Color32 AmbientLightColor;
        public bool IsDirectionalLightEnabled;
        public Color32 DirectionalLightColor;
        public quaternion DirectionalLightRotation = quaternion.identity;
        public bool IsFogEnabled;
        public Color32 FogColor = Color.black;
        public float FogStartDistance;
        public float FogEndDistance;
        
        public CellLightingInfo Build()
        {
            return new CellLightingInfo(this);
        }
    }
}