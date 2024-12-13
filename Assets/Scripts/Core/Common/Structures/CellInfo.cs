using System.Collections.Generic;
using Unity.Mathematics;

namespace Core.Common.Structures
{
    public class CellInfo
    {
        public readonly GameObject.GameObject RootGameObject;
        
        public readonly List<GameObject.GameObject> UnprocessedGameObjects = new();
        
        public CellOcclusionInfo OcclusionInfo;
        
        public float3 DefaultSpawnPosition;
        
        public quaternion DefaultSpawnRotation;
        
        public CellInfo(GameObject.GameObject rootGameObject)
        {
            RootGameObject = rootGameObject;
        }
    }
}