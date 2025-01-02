using Core.Common;
using Core.Common.Converter;
using Core.Common.Structures;
using Core.MasterFile.Converter.Cell.Delegate.Base;
using Core.MasterFile.Manager;
using Core.MasterFile.Manager.Extensions;
using Core.MasterFile.Parser.Structures.Records;
using Unity.Mathematics;
using UnityEngine;

namespace Core.MasterFile.Converter.Cell.Delegate
{
    public class CellLightingDelegate : ICellDelegate
    {
        private const ushort InteriorCellFlagMask = 0x0001;

        private const uint InheritAmbientColorFlagMask = 0x0001;
        private const uint InheritDirectionalColorFlagMask = 0x0002;
        private const uint InheritFogColorFlagMask = 0x0004;
        private const uint InheritFogNearFlagMask = 0x0008;
        private const uint InheritFogFarFlagMask = 0x0010;
        private const uint InheritDirectionalRotFlagMask = 0x0020;
        
        private readonly MasterFileManager _masterFileManager;

        public CellLightingDelegate(MasterFileManager masterFileManager)
        {
            _masterFileManager = masterFileManager;
        }

        public void ProcessCell(RawCellData rawCellData, CellInfoBuilder resultBuilder)
        {
            if (!Utils.IsFlagSet(rawCellData.CellRecord.CellFlag, InteriorCellFlagMask)) return;

            var templateLightingInfo = _masterFileManager
                .GetFromFormId<LGTM>(rawCellData.CellRecord.LightingTemplateFormId)
                ?.LightingInfo;
            if (templateLightingInfo == null) return;

            var cellLightingInfo = rawCellData.CellRecord.LightingInfo;

            //TODO color alpha should be 255?
            if (cellLightingInfo != null &&
                !Utils.IsFlagSet(cellLightingInfo.InheritFlags, InheritAmbientColorFlagMask))
            {
                resultBuilder.LightingInfoBuilder.AmbientLightColor = new Color32(
                    cellLightingInfo.AmbientColor.R, cellLightingInfo.AmbientColor.G,
                    cellLightingInfo.AmbientColor.B, cellLightingInfo.AmbientColor.A);
            }
            else
            {
                resultBuilder.LightingInfoBuilder.AmbientLightColor = new Color32(
                    templateLightingInfo.AmbientColor.R, templateLightingInfo.AmbientColor.G,
                    templateLightingInfo.AmbientColor.B, templateLightingInfo.AmbientColor.A);
            }

            if (cellLightingInfo != null &&
                !Utils.IsFlagSet(cellLightingInfo.InheritFlags, InheritDirectionalColorFlagMask))
            {
                resultBuilder.LightingInfoBuilder.DirectionalLightColor = new Color32(
                    cellLightingInfo.DirectionalColor.R, cellLightingInfo.DirectionalColor.G,
                    cellLightingInfo.DirectionalColor.B, cellLightingInfo.DirectionalColor.A);
            }
            else
            {
                resultBuilder.LightingInfoBuilder.DirectionalLightColor = new Color32(
                    templateLightingInfo.DirectionalColor.R, templateLightingInfo.DirectionalColor.G,
                    templateLightingInfo.DirectionalColor.B, templateLightingInfo.DirectionalColor.A);
            }

            if (resultBuilder.LightingInfoBuilder.DirectionalLightColor != Color.black)
            {
                resultBuilder.LightingInfoBuilder.IsDirectionalLightEnabled = true;
                if (cellLightingInfo != null &&
                    !Utils.IsFlagSet(cellLightingInfo.InheritFlags, InheritDirectionalRotFlagMask))
                {
                    TransformConverter.SkyrimRadiansToUnityQuaternion(
                        new float3(cellLightingInfo.DirectionalRotationXY, cellLightingInfo.DirectionalRotationXY,
                            cellLightingInfo.DirectionalRotationZ),
                        out resultBuilder.LightingInfoBuilder.DirectionalLightRotation);
                }
                else
                {
                    TransformConverter.SkyrimRadiansToUnityQuaternion(
                        new float3(templateLightingInfo.DirectionalRotationXY,
                            templateLightingInfo.DirectionalRotationXY,
                            templateLightingInfo.DirectionalRotationZ),
                        out resultBuilder.LightingInfoBuilder.DirectionalLightRotation);
                }
            }
            else
            {
                resultBuilder.LightingInfoBuilder.IsDirectionalLightEnabled = false;
            }

            if (cellLightingInfo != null && !Utils.IsFlagSet(cellLightingInfo.InheritFlags, InheritFogFarFlagMask))
            {
                resultBuilder.LightingInfoBuilder.FogEndDistance =
                    cellLightingInfo.FogFar / Constants.MeterInSkyrimUnits;
            }
            else
            {
                resultBuilder.LightingInfoBuilder.FogEndDistance =
                    templateLightingInfo.FogFar / Constants.MeterInSkyrimUnits;
            }

            resultBuilder.LightingInfoBuilder.IsFogEnabled = resultBuilder.LightingInfoBuilder.FogEndDistance > 0;
            if (resultBuilder.LightingInfoBuilder.IsFogEnabled && cellLightingInfo != null &&
                !Utils.IsFlagSet(cellLightingInfo.InheritFlags, InheritFogNearFlagMask))
            {
                resultBuilder.LightingInfoBuilder.FogStartDistance =
                    cellLightingInfo.FogNear / Constants.MeterInSkyrimUnits;
            }
            else if (resultBuilder.LightingInfoBuilder.IsFogEnabled)
            {
                resultBuilder.LightingInfoBuilder.FogStartDistance =
                    templateLightingInfo.FogNear / Constants.MeterInSkyrimUnits;
            }

            if (resultBuilder.LightingInfoBuilder.IsFogEnabled && cellLightingInfo != null &&
                !Utils.IsFlagSet(cellLightingInfo.InheritFlags, InheritFogColorFlagMask))
            {
                resultBuilder.LightingInfoBuilder.FogColor = new Color32(cellLightingInfo.FogFarColor.R,
                    cellLightingInfo.FogFarColor.G, cellLightingInfo.FogFarColor.B,
                    cellLightingInfo.FogFarColor.A);
            }
            else if (resultBuilder.LightingInfoBuilder.IsFogEnabled)
            {
                resultBuilder.LightingInfoBuilder.FogColor = new Color32(templateLightingInfo.FogFarColor.R,
                    templateLightingInfo.FogFarColor.G, templateLightingInfo.FogFarColor.B,
                    templateLightingInfo.FogFarColor.A);
            }
        }
    }
}