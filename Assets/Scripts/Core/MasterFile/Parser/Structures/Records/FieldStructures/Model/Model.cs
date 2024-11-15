using System.Collections.Generic;

namespace Core.MasterFile.Parser.Structures.Records.FieldStructures.Model
{
    /// <summary>
    /// Model info with path and potential alternate textures
    /// (Represents a MODL field)
    /// </summary>
    public class Model
    {
        public readonly string FilePath;

        public readonly List<AlternateTexture> AlternateTextures;

        public Model(string filePath, List<AlternateTexture> alternateTextures = null)
        {
            FilePath = filePath;
            AlternateTextures = alternateTextures;
        }
    }
}