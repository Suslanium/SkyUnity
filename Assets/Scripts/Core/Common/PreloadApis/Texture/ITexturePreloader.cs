namespace Core.Common.PreloadApis.Texture
{
    public interface ITexturePreloader
    {
        public void PreloadTexture(TextureType textureType, string texturePath);
    }
}