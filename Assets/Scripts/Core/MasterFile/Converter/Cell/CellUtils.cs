using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Core.Common.Converter;
using Core.Common.GameObject;
using Core.Common.GameObject.Components.Mesh;
using Core.MasterFile.Manager;
using Core.MasterFile.Parser.Structures.Records;
using Core.MasterFile.Parser.Structures.Records.FieldStructures.Model;
using Unity.Mathematics;

namespace Core.MasterFile.Converter.Cell
{
    public static class CellUtils
    {
        private static readonly float3 One = new(1, 1, 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ApplyPositionAndRotation(float3 position, float3 rotation, float scale, GameObject gameObj)
        {
            if (gameObj == null) return;

            if (scale != 0f)
            {
                gameObj.LocalScale = scale * One;
            }

            TransformConverter.SkyrimPointToUnityPoint(position, out gameObj.Position);
            TransformConverter.SkyrimRadiansToUnityQuaternion(rotation, out gameObj.Rotation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MeshInfo ToMeshInfo(this Model model, MasterFileManager masterFileManager)
        {
            var alternateTextures = new List<AlternateTextureInfo>();
            foreach (var alternateTexture in model.AlternateTextures)
            {
                if (alternateTexture.TextureSetFormID == 0) continue;

                var textureRecord = masterFileManager.GetFromFormId<TXST>(alternateTexture.TextureSetFormID);
                if (textureRecord == null) continue;

                var alternateTextureInfo = new AlternateTextureInfo(
                    alternateTexture.ObjectName,
                    alternateTexture.Index,
                    textureRecord.DiffuseMapPath,
                    textureRecord.NormalMapPath,
                    textureRecord.MaskMapPath,
                    textureRecord.GlowMapPath,
                    textureRecord.DetailMapPath,
                    textureRecord.EnvironmentMapPath,
                    textureRecord.MultiLayerMapPath,
                    textureRecord.SpecularMapPath);
                alternateTextures.Add(alternateTextureInfo);
            }

            return new MeshInfo(model.FilePath, alternateTextures);
        }
    }
}