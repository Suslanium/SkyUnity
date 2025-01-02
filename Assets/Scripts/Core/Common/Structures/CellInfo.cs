using System.Collections.Generic;
using Unity.Mathematics;

namespace Core.Common.Structures
{
    public class CellInfo
    {
        public readonly GameObject.GameObject RootGameObject;
        
        public readonly IReadOnlyList<GameObject.GameObject> UnprocessedGameObjects;
        
        public readonly CellOcclusionInfo OcclusionInfo;
        
        public readonly CellLightingInfo LightingInfo;
        
        public readonly float3? DefaultSpawnPosition;
        
        public readonly quaternion? DefaultSpawnRotation;
        
        public CellInfo(CellInfoBuilder builder)
        {
            RootGameObject = builder.RootGameObject;
            UnprocessedGameObjects = builder.UnprocessedGameObjects;
            OcclusionInfo = builder.OcclusionInfoBuilder.Build();
            LightingInfo = builder.LightingInfoBuilder.Build();
            DefaultSpawnPosition = builder.DefaultSpawnPosition;
            DefaultSpawnRotation = builder.DefaultSpawnRotation;
        }
    }

    public class CellInfoBuilder
    {
        public readonly GameObject.GameObject RootGameObject;
        
        public readonly List<GameObject.GameObject> UnprocessedGameObjects = new();
        
        public readonly CellOcclusionInfoBuilder OcclusionInfoBuilder = new();
        
        public readonly CellLightingInfoBuilder LightingInfoBuilder = new();
        
        public float3? DefaultSpawnPosition;
        
        public quaternion? DefaultSpawnRotation;
        
        public CellInfoBuilder(GameObject.GameObject rootGameObject)
        {
            RootGameObject = rootGameObject;
        }
        
        public CellInfo Build()
        {
            return new CellInfo(this);
        }
    }
}