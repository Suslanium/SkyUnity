namespace Core.Common.GameObject.Components.Mesh
{
    public class UnprocessedMeshComponent : IUnprocessedComponent
    {
        public readonly MeshInfo MeshInfo;
        
        public UnprocessedMeshComponent(MeshInfo meshInfo)
        {
            MeshInfo = meshInfo;
        }
    }
}