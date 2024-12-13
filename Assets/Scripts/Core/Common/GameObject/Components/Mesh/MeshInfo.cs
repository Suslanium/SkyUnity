using System.Collections.Generic;

namespace Core.Common.GameObject.Components.Mesh
{
    public class MeshInfo
    {
        public readonly string FilePath;
        
        public readonly IReadOnlyCollection<AlternateTextureInfo> AlternateTextures;
        
        public MeshInfo(string filePath, IReadOnlyCollection<AlternateTextureInfo> alternateTextures)
        {
            FilePath = filePath;
            AlternateTextures = alternateTextures;
        }
    }
}