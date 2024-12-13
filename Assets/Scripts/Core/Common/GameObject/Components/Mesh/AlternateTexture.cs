namespace Core.Common.GameObject.Components.Mesh
{
    public class AlternateTextureInfo
    {
        /// <summary>
        /// 3D object name inside the model file
        /// </summary>
        public readonly string ObjectName;

        /// <summary>
        /// 3D index (index of a NiNode???)
        /// </summary>
        public readonly uint Index;

        public readonly string DiffuseMapPath;

        public readonly string NormalMapPath;

        public readonly string MaskMapPath;

        public readonly string GlowMapPath;

        public readonly string DetailMapPath;

        public readonly string EnvironmentMapPath;

        public readonly string MultiLayerMapPath;

        public readonly string SpecularMapPath;

        public AlternateTextureInfo(string objectName, uint index, string diffuseMapPath, string normalMapPath,
            string maskMapPath, string glowMapPath, string detailMapPath, string environmentMapPath,
            string multiLayerMapPath, string specularMapPath)
        {
            ObjectName = objectName;
            Index = index;
            DiffuseMapPath = diffuseMapPath;
            NormalMapPath = normalMapPath;
            MaskMapPath = maskMapPath;
            GlowMapPath = glowMapPath;
            DetailMapPath = detailMapPath;
            EnvironmentMapPath = environmentMapPath;
            MultiLayerMapPath = multiLayerMapPath;
            SpecularMapPath = specularMapPath;
        }
    }
}