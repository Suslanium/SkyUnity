using System.IO;
using Core.MasterFile.Parser.Structures.Records.FieldStructures;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.Model;

namespace Core.MasterFile.Parser.Reader.RecordTypeReaders
{
    public static class FieldReaderUtils
    {
        private const string ModelPathField = "MODL";
        private const string AlternateTextureField = "MODS";
        
        public static bool TryReadModelField(this BinaryReader fileReader, ModelBuilder builder, FieldInfo fieldInfo)
        {
            switch (fieldInfo.Type)
            {
                case ModelPathField:
                    builder.FilePath = fileReader.ReadZString(fieldInfo.Size);
                    return true;
                case AlternateTextureField:
                    var count = fileReader.ReadUInt32();
                    for (var i = 0; i < count; i++)
                    {
                        var objectNameLength = checked((int)fileReader.ReadUInt32());
                        var objectName = fileReader.ReadString(objectNameLength);
                        var textureSetFormID = fileReader.ReadFormId();
                        var index = fileReader.ReadUInt32();
                        builder.AlternateTextures.Add(new AlternateTexture(objectName, index, textureSetFormID));
                    }
                    return true;
                default:
                    return false;
            }
        }

        public static Lighting ReadLightingField(this BinaryReader fileReader, int fieldSize)
        {
            if (fieldSize is 92 or 64)
            {
                var builder = new LightingBuilder
                {
                    AmbientColor = fileReader.ReadColorRGBA(),
                    DirectionalColor = fileReader.ReadColorRGBA(),
                    FogNearColor = fileReader.ReadColorRGBA(),
                    FogNear = fileReader.ReadFloat32(),
                    FogFar = fileReader.ReadFloat32(),
                    DirectionalRotationXY = fileReader.ReadInt32(),
                    DirectionalRotationZ = fileReader.ReadInt32(),
                    DirectionalFade = fileReader.ReadFloat32()
                };

                if (fieldSize == 64)
                {
                    fileReader.BaseStream.Seek(32, SeekOrigin.Current);
                    return builder.Build();
                }

                fileReader.BaseStream.Seek(40, SeekOrigin.Current);
                builder.FogFarColor = fileReader.ReadColorRGBA();
                builder.FogMax = fileReader.ReadFloat32();
                builder.LightFadeDistanceStart = fileReader.ReadFloat32();
                builder.LightFadeDistanceEnd = fileReader.ReadFloat32();
                builder.InheritFlags = fileReader.ReadUInt32();
                return builder.Build();
            }
            else
            {
                fileReader.BaseStream.Seek(fieldSize, SeekOrigin.Current);
                return null;
            }
        }

        public static ObjectBounds ReadObjectBounds(this BinaryReader fileReader)
        {
            return new ObjectBounds(
                fileReader.ReadInt16(),
                fileReader.ReadInt16(),
                fileReader.ReadInt16(),
                fileReader.ReadInt16(),
                fileReader.ReadInt16(),
                fileReader.ReadInt16());
        }
    }
}