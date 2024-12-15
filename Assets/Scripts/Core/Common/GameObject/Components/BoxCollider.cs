using Unity.Mathematics;

namespace Core.Common.GameObject.Components
{
    public class BoxCollider : IComponent
    {
        public readonly bool IsTrigger;
        public readonly float3 Center;
        public readonly float3 Size;
        
        public BoxCollider(bool isTrigger, float3 center, float3 size)
        {
            IsTrigger = isTrigger;
            Center = center;
            Size = size;
        }
        
        public BoxCollider(bool isTrigger, float3 size)
        {
            IsTrigger = isTrigger;
            Center = float3.zero;
            Size = size;
        }
    }
}