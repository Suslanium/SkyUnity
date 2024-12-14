using System.Collections.Generic;

namespace Core.Common.GameObject.Components.Mesh
{
    public class MeshInfo
    {
        public readonly string FilePath;
        
        public readonly IReadOnlyList<AlternateTextureInfo> AlternateTextures;
        
        public MeshInfo(string filePath, IReadOnlyList<AlternateTextureInfo> alternateTextures)
        {
            FilePath = filePath;
            AlternateTextures = alternateTextures;
        }
    }
}