using System.IO;
using Core.Common;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.General;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.Landscape;

namespace Core.MasterFile.Parser.Reader.RecordTypeReaders
{
    // ReSharper disable once InconsistentNaming
    public class LANDReader : RecordTypeReader<LANDBuilder>
    {
        private const string RecordType = "LAND";
        private const string HeightMapField = "VHGT";
        private const int HeightMultiplier = 8;
        private const int LandSideLength = Constants.SkyrimExteriorCellSideLengthInSamples;
        private const int LandQuadrantSideLength = Constants.SkyrimExteriorCellQuadrantSideLengthInSamples;
        private const string VertexColorField = "VCLR";
        private const string BaseTextureField = "BTXT";
        private const string AdditionalTextureField = "ATXT";
        private const string AdditionalTextureAlphaMapField = "VTXT";

        public override string GetRecordType()
        {
            return RecordType;
        }

        //More info about the structure of LAND records can be found here:
        //For the heightmap: https://en.uesp.net/wiki/Skyrim_Mod:Mod_File_Format/LAND
        //For the vertex colors, layers, etc: https://en.uesp.net/wiki/Oblivion_Mod:Mod_File_Format/LAND
        protected override void ReadField(
            MasterFileProperties properties,
            BinaryReader fileReader,
            FieldInfo fieldInfo,
            LANDBuilder builder)
        {
            switch (fieldInfo.Type)
            {
                case HeightMapField:
                    var vertexHeightMap = new float[LandSideLength, LandSideLength];

                    var heightOffset = fileReader.ReadFloat32() * HeightMultiplier;
                    var rowHeightOffset = 0;

                    for (var i = 0; i < LandSideLength * LandSideLength; i++)
                    {
                        var rawHeightValue = fileReader.ReadSByte() * HeightMultiplier;

                        var currentRow = i / LandSideLength;
                        var currentColumn = i % LandSideLength;

                        if (currentColumn == 0)
                        {
                            rowHeightOffset = 0;
                            heightOffset += rawHeightValue;
                        }
                        else
                        {
                            rowHeightOffset += rawHeightValue;
                        }

                        vertexHeightMap[currentRow, currentColumn] = heightOffset + rowHeightOffset;
                    }

                    builder.VertexHeightMap = vertexHeightMap;
                    fileReader.BaseStream.Seek(3, SeekOrigin.Current);
                    break;
                case VertexColorField:
                    var vertexColors = new ColorRGB[LandSideLength, LandSideLength];

                    for (var i = 0; i < LandSideLength * LandSideLength; i++)
                    {
                        var color = fileReader.ReadByteColorRGB();

                        var currentRow = i / LandSideLength;
                        var currentColumn = i % LandSideLength;

                        vertexColors[currentRow, currentColumn] = color;
                    }

                    builder.VertexColors = vertexColors;
                    break;
                case BaseTextureField:
                    var landTextureFormId = fileReader.ReadFormId();
                    var quadrant = fileReader.ReadByte();
                    fileReader.BaseStream.Seek(3, SeekOrigin.Current);

                    builder.BaseTextures.Add(new BaseTextureLayer(landTextureFormId, quadrant));
                    break;
                case AdditionalTextureField:
                    var additionalLandTextureFormId = fileReader.ReadFormId();
                    var additionalTextureQuadrant = fileReader.ReadByte();
                    fileReader.BaseStream.Seek(1, SeekOrigin.Current);
                    var layerIndex = fileReader.ReadUInt16();
                    var quadrantAlphaMap = new float[LandQuadrantSideLength, LandQuadrantSideLength];

                    builder.AdditionalTextures.Add(new AdditionalTextureLayer(additionalLandTextureFormId,
                        additionalTextureQuadrant, layerIndex, quadrantAlphaMap));
                    break;
                case AdditionalTextureAlphaMapField:
                    for (var currentByte = 0; currentByte < fieldInfo.Size; currentByte += 8)
                    {
                        var texturePosition = fileReader.ReadUInt16();
                        fileReader.BaseStream.Seek(2, SeekOrigin.Current);
                        var alphaAtCurrentPosition = fileReader.ReadFloat32();

                        var currentRow = texturePosition / LandQuadrantSideLength;
                        var currentColumn = texturePosition % LandQuadrantSideLength;

                        builder.AdditionalTextures[^1].QuadrantAlphaMap[currentRow, currentColumn] =
                            alphaAtCurrentPosition;
                    }
                    break;
                default:
                    fileReader.BaseStream.Seek(fieldInfo.Size, SeekOrigin.Current);
                    break;
            }
        }
    }
}