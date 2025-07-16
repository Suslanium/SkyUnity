using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Core.Common;
using Core.Common.Structures;
using Core.MasterFile.Converter.Cell.Delegate.Base;
using Core.MasterFile.Manager;
using Core.MasterFile.Manager.Extensions;
using Core.MasterFile.Parser.Structures.Records;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using GameObject = Core.Common.GameObject.GameObject;
using Terrain = Core.Common.GameObject.Components.Terrain;
using TerrainLayer = Core.Common.GameObject.Components.TerrainLayer;

namespace Core.MasterFile.Converter.Cell.Delegate
{
    public enum Quadrant
    {
        BottomLeft = 0,
        BottomRight = 1,
        TopLeft = 2,
        TopRight = 3,
    }

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

    public readonly struct RawTextureLayerInfo
    {
        public readonly uint TextureSetFormId;

        public readonly Quadrant Quadrant;

        /// <summary>
        /// Null alpha map means that the layer is fully opaque.
        /// </summary>
        [CanBeNull] public readonly float[,] LayerAlphaMap;

        public RawTextureLayerInfo(uint textureSetFormId, Quadrant quadrant, [CanBeNull] float[,] layerAlphaMap)
        {
            TextureSetFormId = textureSetFormId;
            Quadrant = quadrant;
            LayerAlphaMap = layerAlphaMap;
        }
    }

    public readonly struct RawMergedTextureLayerInfo
    {
        public readonly uint TextureSetFormId;

        /// <summary>
        /// Null list means that the layer is fully opaque.
        /// </summary>
        public readonly Dictionary<Quadrant, List<float[,]>> QuadrantAlphaMaps;

        public RawMergedTextureLayerInfo(uint textureSetFormId, Dictionary<Quadrant, List<float[,]>> quadrantAlphaMaps)
        {
            TextureSetFormId = textureSetFormId;
            QuadrantAlphaMaps = quadrantAlphaMaps;
        }
    }

    public readonly struct ConvertedTextureLayers
    {
        public readonly uint[] TextureSetFormIds;

        public readonly float[,,] AlphaMaps;

        public ConvertedTextureLayers(uint[] textureSetFormIds, float[,,] alphaMaps)
        {
            TextureSetFormIds = textureSetFormIds;
            AlphaMaps = alphaMaps;
        }
    }

    public class CellTerrainDelegate : CellRecordDelegate<LAND>
    {
        private const int AlphaMapResolution = 128;
        private const int TerrainQuadrantResolution = AlphaMapResolution / 2;
        private const int QuadrantRawAlphaMapResolution = Constants.SkyrimExteriorCellQuadrantSideLengthInSamples;
        private const int LandSideLengthInSamples = Constants.SkyrimExteriorCellSideLengthInSamples;

        private const float TerrainWidth =
            (LandSideLengthInSamples - 1) *
            (Constants.SkyrimExteriorCellSideLengthInMeters / (LandSideLengthInSamples - 1));

        private const string TexturePathPrefix = "Textures";
        private const string DefaultDiffuseTexturePath = "Textures/Landscape/Dirt02.dds";
        private const string DefaultNormalMapPath = "Textures/Landscape/Dirt02_n.dds";

        private readonly MasterFileManager _masterFileManager;

        private static List<RawTextureLayerInfo>[] GetRawLayerList(LAND record)
        {
            var rawLayerList = new List<RawTextureLayerInfo>[4];

            // Each index corresponds to a quadrant.
            for (var i = 0; i < 4; i++)
            {
                rawLayerList[i] = new List<RawTextureLayerInfo>();
            }

            var missingQuadrants = new HashSet<Quadrant>
            {
                Quadrant.BottomLeft,
                Quadrant.BottomRight,
                Quadrant.TopLeft,
                Quadrant.TopRight
            };

            foreach (var baseLayer in record.BaseTextures)
            {
                var quadrant = (Quadrant)baseLayer.Quadrant;
                var layer = new RawTextureLayerInfo(baseLayer.LandTextureFormID, quadrant, null);
                rawLayerList[(int)quadrant].Add(layer);
                missingQuadrants.Remove(quadrant);
            }

            foreach (var quadrant in missingQuadrants)
            {
                rawLayerList[(int)quadrant].Add(new RawTextureLayerInfo(0, quadrant, null));
            }

            foreach (var additionalLayer in record.AdditionalTextures)
            {
                var quadrant = (Quadrant)additionalLayer.Quadrant;
                var layer = new RawTextureLayerInfo(additionalLayer.LandTextureFormID, quadrant,
                    additionalLayer.QuadrantAlphaMap);
                rawLayerList[(int)quadrant].Add(layer);
            }

            return rawLayerList;
        }

        private static List<RawMergedTextureLayerInfo> MergeTextureLayers(List<RawTextureLayerInfo>[] layers)
        {
            var layerCountPerQuadrant = layers.Select(list => list.Count).ToArray();
            var indexLimit = layerCountPerQuadrant.Max();
            //Indices to min layer count memo
            var memoizedLayerCount = new Dictionary<(int, int, int, int), int>();
            //Self-managed stack to avoid potential stack overflow
            //Indices, Current merged layers, current merged layer count and advance value
            var stack = new Stack<(int, int, int, int, List<RawMergedTextureLayerInfo>, int, int)>();

            var bestResultListSize = int.MaxValue;
            List<RawMergedTextureLayerInfo> bestResultList = null;
            stack.Push((0, 0, 0, 0, new List<RawMergedTextureLayerInfo>(), 0, 16));

            while (stack.Count > 0)
            {
                var (i0, i1, i2, i3, currentList, currentCount, previousAdvance) = stack.Pop();

                if (i0 > indexLimit || i1 > indexLimit || i2 > indexLimit || i3 > indexLimit)
                {
                    continue;
                }

                if (i0 >= layerCountPerQuadrant[0] && i1 >= layerCountPerQuadrant[1] &&
                    i2 >= layerCountPerQuadrant[2] && i3 >= layerCountPerQuadrant[3])
                {
                    if (currentCount < bestResultListSize)
                    {
                        bestResultList = currentList;
                        bestResultListSize = currentCount;
                    }

                    continue;
                }

                if (!memoizedLayerCount.TryGetValue((i0, i1, i2, i3), out var memoizedCount))
                {
                    memoizedLayerCount[(i0, i1, i2, i3)] = currentCount;
                }
                else
                {
                    if (memoizedCount <= currentCount)
                    {
                        continue;
                    }

                    memoizedLayerCount[(i0, i1, i2, i3)] = currentCount;
                }

                var newLayers = new List<RawTextureLayerInfo>();
                if (i0 < layerCountPerQuadrant[0] && (previousAdvance & 1) > 0)
                {
                    newLayers.Add(layers[0][i0]);
                }

                if (i1 < layerCountPerQuadrant[1] && (previousAdvance & 2) > 0)
                {
                    newLayers.Add(layers[1][i1]);
                }

                if (i2 < layerCountPerQuadrant[2] && (previousAdvance & 4) > 0)
                {
                    newLayers.Add(layers[2][i2]);
                }

                if (i3 < layerCountPerQuadrant[3] && (previousAdvance & 8) > 0)
                {
                    newLayers.Add(layers[3][i3]);
                }

                var mergedLayers = new Dictionary<uint, Dictionary<Quadrant, List<float[,]>>>();
                foreach (var layer in newLayers)
                {
                    if (!mergedLayers.TryGetValue(layer.TextureSetFormId, out var quadrantAlphaMaps))
                    {
                        quadrantAlphaMaps = new Dictionary<Quadrant, List<float[,]>>();
                        mergedLayers[layer.TextureSetFormId] = quadrantAlphaMaps;
                    }

                    if (!quadrantAlphaMaps.ContainsKey(layer.Quadrant))
                    {
                        quadrantAlphaMaps[layer.Quadrant] = layer.LayerAlphaMap != null
                            ? new List<float[,]>
                            {
                                layer.LayerAlphaMap
                            }
                            : null;
                    }
                    else
                    {
                        if (layer.LayerAlphaMap == null)
                        {
                            quadrantAlphaMaps[layer.Quadrant] = null;
                        }
                        else
                        {
                            quadrantAlphaMaps[layer.Quadrant]?.Add(layer.LayerAlphaMap);
                        }
                    }
                }

                var currentMergedLayers =
                    mergedLayers.Select(layer => new RawMergedTextureLayerInfo(layer.Key, layer.Value));

                var newCurrentLayerList = new List<RawMergedTextureLayerInfo>(currentList);
                newCurrentLayerList.AddRange(currentMergedLayers);

                for (var advance = 0; advance < 16; advance++)
                {
                    var ni0 = i0 + ((advance & 1) > 0 ? 1 : 0);
                    var ni1 = i1 + ((advance & 2) > 0 ? 1 : 0);
                    var ni2 = i2 + ((advance & 4) > 0 ? 1 : 0);
                    var ni3 = i3 + ((advance & 8) > 0 ? 1 : 0);
                    stack.Push((ni0, ni1, ni2, ni3, newCurrentLayerList, newCurrentLayerList.Count, advance));
                }
            }

            return bestResultList ?? new List<RawMergedTextureLayerInfo>();
        }

        private static ConvertedTextureLayers ConvertMergedLayers(List<RawMergedTextureLayerInfo> mergedLayers)
        {
            var mergedAlphaMaps = new float[AlphaMapResolution, AlphaMapResolution, mergedLayers.Count];
            var textureFormIDs = new uint[mergedLayers.Count];

            var mergeTasks = new Task[mergedLayers.Count - 1];

            for (var i = 0; i < mergedLayers.Count - 1; i++)
            {
                var currentLayer = mergedLayers[i];
                var currentLayerIndex = i;
                mergeTasks[i] = Task.Run(() =>
                    WriteMergedTextureLayer(currentLayer, textureFormIDs, mergedAlphaMaps, currentLayerIndex));
            }

            //Merge the last layer on the current thread to avoid wasting resources
            WriteMergedTextureLayer(mergedLayers[^1], textureFormIDs, mergedAlphaMaps, mergedLayers.Count - 1);

            Task.WaitAll(mergeTasks);

            return new ConvertedTextureLayers(textureFormIDs, mergedAlphaMaps);
        }

        private static Quadrant GetQuadrant(int x, int y)
        {
            if (x < AlphaMapResolution / 2 && y < AlphaMapResolution / 2)
                return Quadrant.BottomLeft;
            if (x >= AlphaMapResolution / 2 && y < AlphaMapResolution / 2)
                return Quadrant.TopLeft;
            if (x < AlphaMapResolution / 2 && y >= AlphaMapResolution / 2)
                return Quadrant.BottomRight;
            if (x >= AlphaMapResolution / 2 && y >= AlphaMapResolution / 2)
                return Quadrant.TopRight;
            throw new ArgumentException("Invalid coordinates for alpha map quadrant determination");
        }

        private static void WriteMergedTextureLayer(RawMergedTextureLayerInfo mergedTerrainLayerInfo,
            uint[] textureFormIDs, float[,,] resultingAlphaMaps, int layerIndex)
        {
            for (var y = 0; y < AlphaMapResolution; y++)
            {
                for (var x = 0; x < AlphaMapResolution; x++)
                {
                    //Determine the current quadrant
                    var quadrant = GetQuadrant(x, y);
                    //Get the alpha maps for the current quadrant
                    if (!mergedTerrainLayerInfo.QuadrantAlphaMaps.TryGetValue(quadrant, out var alphaMaps))
                        continue;

                    if (alphaMaps == null)
                    {
                        //Cover the entire quadrant
                        resultingAlphaMaps[x, y, layerIndex] = 1;
                        continue;
                    }

                    //Get raw alpha map coordinates
                    var qx = (float)(x % TerrainQuadrantResolution) / TerrainQuadrantResolution *
                             QuadrantRawAlphaMapResolution;
                    var qy = (float)(y % TerrainQuadrantResolution) / TerrainQuadrantResolution *
                             QuadrantRawAlphaMapResolution;

                    var xLess = Mathf.Max(0, Mathf.FloorToInt(qx));
                    var xMore = Mathf.Min(QuadrantRawAlphaMapResolution - 1, Mathf.CeilToInt(qx));
                    var xFractional = qx - xLess;

                    var yLess = Mathf.Max(0, Mathf.FloorToInt(qy));
                    var yMore = Mathf.Min(QuadrantRawAlphaMapResolution - 1, Mathf.CeilToInt(qy));
                    var yFractional = qy - yLess;

                    var topLeftWeight = (1 - xFractional) * (1 - yFractional);
                    var topRightWeight = xFractional * (1 - yFractional);
                    var bottomLeftWeight = (1 - xFractional) * yFractional;
                    var bottomRightWeight = xFractional * yFractional;

                    float valueSum = 0;
                    foreach (var alphaMap in alphaMaps)
                    {
                        var topLeft = alphaMap[xLess, yLess];
                        var topRight = alphaMap[xMore, yLess];
                        var bottomLeft = alphaMap[xLess, yMore];
                        var bottomRight = alphaMap[xMore, yMore];
                        valueSum += topLeft * topLeftWeight + topRight * topRightWeight +
                                    bottomLeft * bottomLeftWeight + bottomRight * bottomRightWeight;
                    }

                    resultingAlphaMaps[x, y, layerIndex] = Mathf.Clamp01(valueSum);
                }
            }

            textureFormIDs[layerIndex] = mergedTerrainLayerInfo.TextureSetFormId;
        }

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
                ? new TerrainMeshInfo(new float3(TerrainWidth, maxHeightInMeters, TerrainWidth), heightMap,
                    minHeight, maxHeight)
                : new TerrainMeshInfo(new float3(TerrainWidth, 1, TerrainWidth), null, minHeight, maxHeight);
        }

        private (string, string) GetTexturePaths(uint landTextureFormId)
        {
            if (landTextureFormId == 0)
            {
                return (DefaultDiffuseTexturePath, DefaultNormalMapPath);
            }

            var landTextureRecord = _masterFileManager.GetFromFormId<LTEX>(landTextureFormId);
            if (landTextureRecord == null || landTextureRecord.TextureFormID == 0)
            {
                return (DefaultDiffuseTexturePath, DefaultNormalMapPath);
            }

            var textureSetRecord = _masterFileManager.GetFromFormId<TXST>(landTextureRecord.TextureFormID);
            if (textureSetRecord == null)
            {
                return (DefaultDiffuseTexturePath, DefaultNormalMapPath);
            }

            var diffuseMapPath = textureSetRecord.DiffuseMapPath;
            var normalMapPath = textureSetRecord.NormalMapPath;
            if (string.IsNullOrEmpty(diffuseMapPath))
            {
                return (DefaultDiffuseTexturePath, DefaultNormalMapPath);
            }

            if (!diffuseMapPath.StartsWith(TexturePathPrefix, ignoreCase: true, CultureInfo.InvariantCulture))
                diffuseMapPath = $"{TexturePathPrefix}/{diffuseMapPath}";
            diffuseMapPath = diffuseMapPath.Replace('\\', '/');

            if (normalMapPath != null &&
                !normalMapPath.StartsWith(TexturePathPrefix, ignoreCase: true, CultureInfo.InvariantCulture))
                normalMapPath = $"{TexturePathPrefix}/{normalMapPath}";
            normalMapPath = normalMapPath?.Replace('\\', '/');

            return (diffuseMapPath, normalMapPath);
        }
        
        public CellTerrainDelegate(MasterFileManager masterFileManager)
        {
            _masterFileManager = masterFileManager;
        }

        public bool IsConcurrent => true;

        protected override void ProcessRecord(RawCellData rawCellData, LAND record, CellInfoBuilder resultBuilder)
        {
            var terrainMeshInfo = GetTerrainMeshInfo(record.VertexHeightMap);
            var rawLayers = GetRawLayerList(record);
            var mergedLayers = MergeTextureLayers(rawLayers);
            var convertedLayers = ConvertMergedLayers(mergedLayers);

            var terrainLayers = convertedLayers.TextureSetFormIds.Select(GetTexturePaths).Select(textures =>
                new TerrainLayer(textures.Item1, textures.Item2, float3.zero, new float3(2, 2, 2))).ToArray();
            var terrain = new Terrain(LandSideLengthInSamples, AlphaMapResolution, terrainMeshInfo.Size,
                terrainMeshInfo.HeightMap, convertedLayers.AlphaMaps, terrainLayers);
            
            var gameObject = new GameObject("Terrain", resultBuilder.RootGameObject);
            resultBuilder.UnprocessedGameObjects.Add(gameObject);
            gameObject.Components.Add(terrain);
            gameObject.Position = new float3(
                Constants.SkyrimExteriorCellSideLengthInMeters * rawCellData.CellRecord.XGridPosition,
                terrainMeshInfo.MinHeight / Constants.MeterInSkyrimUnits,
                Constants.SkyrimExteriorCellSideLengthInMeters * rawCellData.CellRecord.YGridPosition);
        }
    }
}