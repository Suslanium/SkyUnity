namespace Core.Common.GameObject.Components.Mesh
{
    public class UnprocessedMeshComponent : IComponent
    {
        public readonly MeshInfo MeshInfo;
        
        public UnprocessedMeshComponent(MeshInfo meshInfo)
        {
            MeshInfo = meshInfo;
        }
    }
}