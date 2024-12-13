using Core.Common.GameObject.Components.Mesh;

namespace Core.Common.PreloadApis
{
    public interface IMeshPreloader
    {
        public void PreloadMesh(MeshInfo meshInfo);
    }
}