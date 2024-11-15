namespace Core.MasterFile.Parser.Structures.Records.FieldStructures.Model
{
    public class AlternateTexture
    {
        /// <summary>
        /// 3D object name inside the model file
        /// </summary>
        public readonly string ObjectName;

        /// <summary>
        /// 3D index (index of a NiNode???)
        /// </summary>
        public readonly uint Index;
        
        /// <summary>
        /// TXST record FormID
        /// </summary>
        public readonly uint TextureSetFormID;

        public AlternateTexture(string objectName, uint index, uint textureSetFormID)
        {
            ObjectName = objectName;
            Index = index;
            TextureSetFormID = textureSetFormID;
        }
    }
}