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

        public readonly IReadOnlyList<AlternateTexture> AlternateTextures;

        public Model(ModelBuilder builder)
        {
            FilePath = builder.FilePath;
            AlternateTextures = builder.AlternateTextures;
        }
    }

    public class ModelBuilder
    {
        public string FilePath;
        public List<AlternateTexture> AlternateTextures = new();
        
        public Model Build()
        {
            return new Model(this);
        }
    }
}