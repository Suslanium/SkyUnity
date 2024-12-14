using Core.Common;
using Core.Common.GameObject.Components.Mesh;
using Core.Common.PreloadApis;
using Core.Common.Structures;
using Core.MasterFile.Converter.Cell.Delegate.Reference.Base;
using Core.MasterFile.Manager;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;
using UnityEngine;
using GameObject = Core.Common.GameObject.GameObject;
using Light = Core.Common.GameObject.Components.Light;

namespace Core.MasterFile.Converter.Cell.Delegate.Reference
{
    public class LightObjectDelegate : ICellReferenceDelegate
    {
        private readonly IMeshPreloader _meshPreloader;
        private readonly MasterFileManager _masterFileManager;

        private const uint ShadowSpotLightFlagMask = 0x0400;
        private const uint ShadowHemisphereLightMask = 0x0800;
        private const uint ShadowOmnidirectionalLightMask = 0x1000;

        public LightObjectDelegate(IMeshPreloader meshPreloader, MasterFileManager masterFileManager)
        {
            _meshPreloader = meshPreloader;
            _masterFileManager = masterFileManager;
        }

        public bool IsApplicable(CELL cell, REFR reference, Record referencedRecord)
        {
            return referencedRecord is LIGH;
        }

        public void ProcessReference(CELL cell, REFR reference, Record referencedRecord, CellInfo result)
        {
            if (referencedRecord is not LIGH light) return;
            var gameObj = new GameObject(light.EditorID ?? light.FormId.ToString(), result.RootGameObject);
            
            var modelInfo = light.ModelInfo;
            if (modelInfo != null)
            {
                var meshInfo = modelInfo.ToMeshInfo(_masterFileManager);
                _meshPreloader.PreloadMesh(meshInfo);
                gameObj.Components.Add(new UnprocessedMeshComponent(meshInfo));
                result.UnprocessedGameObjects.Add(gameObj);
            }

            var lightComponent = new Light();
            //TODO this light conversion algorithm is far from perfect
            if (Utils.IsFlagSet(light.Flags, ShadowSpotLightFlagMask))
            {
                var nestedSpotGameObject = new GameObject("SpotLight", gameObj)
                {
                    Rotation = Quaternion.LookRotation(Vector3.down)
                };
                nestedSpotGameObject.Components.Add(lightComponent);
            }
            else
            {
                gameObj.Components.Add(lightComponent);
            }

            lightComponent.Range = 2 * ((light.Radius + reference.Radius) / Constants.MeterInSkyrimUnits);
            //TODO alpha potentially should be 255
            lightComponent.Color = new Color32(light.Color.R, light.Color.G, light.Color.B, light.Color.A);
            lightComponent.Intensity = light.Fade + reference.FadeOffset;
            if (Utils.IsFlagSet(light.Flags, ShadowSpotLightFlagMask))
            {
                lightComponent.Type = LightType.Spot;
            }
            else if (!Utils.IsFlagSet(light.Flags, ShadowHemisphereLightMask) &&
                     !Utils.IsFlagSet(light.Flags, ShadowOmnidirectionalLightMask))
            {
                lightComponent.Shadows = LightShadows.None;
            }

            CellUtils.ApplyPositionAndRotation(reference.Position.XYZ, reference.Rotation.XYZ, reference.Scale, gameObj);
        }
    }
}