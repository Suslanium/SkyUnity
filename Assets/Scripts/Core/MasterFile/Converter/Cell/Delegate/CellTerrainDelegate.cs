using Core.Common;
using Core.Common.Structures;
using Core.MasterFile.Converter.Cell.Delegate.Base;
using Core.MasterFile.Manager.Extensions;
using Unity.Mathematics;
using UnityEngine;

namespace Core.MasterFile.Converter.Cell.Delegate
{
    public readonly struct TerrainMeshInfo
    {
        public readonly float3 Size;

        public readonly float[,] HeightMap;

        public readonly float MinHeight;

        public readonly float MaxHeight;

        public TerrainMeshInfo(float3 size, float[,] heightMap, float minHeight, float maxHeight)
        {
            Size = size;
            HeightMap = heightMap;
            MinHeight = minHeight;
            MaxHeight = maxHeight;
        }
    }

    public class CellTerrainDelegate : ICellDelegate
    {
        private const int LandSideLengthInSamples = Constants.SkyrimExteriorCellSideLengthInSamples;

        private const float TerrainWidth =
            (LandSideLengthInSamples - 1) *
            (Constants.SkyrimExteriorCellSideLengthInMeters / (LandSideLengthInSamples - 1));

        private static TerrainMeshInfo GetTerrainMeshInfo(float[,] heightMap)
        {
            var minHeight = float.MaxValue;
            var maxHeight = float.MinValue;

            for (var i = 0; i < LandSideLengthInSamples; i++)
            {
                for (var j = 0; j < LandSideLengthInSamples; j++)
                {
                    var height = heightMap[i, j];
                    minHeight = math.min(height, minHeight);
                    maxHeight = math.max(height, maxHeight);
                }
            }

            for (var i = 0; i < LandSideLengthInSamples; i++)
            {
                for (var j = 0; j < LandSideLengthInSamples; j++)
                {
                    heightMap[i, j] = (heightMap[i, j] - minHeight) / (maxHeight - minHeight);
                }
            }

            var heightRange = maxHeight - minHeight;
            var maxHeightInMeters = heightRange / Constants.MeterInSkyrimUnits;

            return !Mathf.Approximately(maxHeightInMeters, 0)
                ? new TerrainMeshInfo(new Vector3(TerrainWidth, maxHeightInMeters, TerrainWidth), heightMap,
                    minHeight, maxHeight)
                : new TerrainMeshInfo(new Vector3(TerrainWidth, 1, TerrainWidth), null, minHeight, maxHeight);
        }

        public void ProcessCell(RawCellData rawCellData, CellInfoBuilder resultBuilder)
        {
        }
    }
}