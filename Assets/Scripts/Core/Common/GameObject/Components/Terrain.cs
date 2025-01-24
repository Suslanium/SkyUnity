using Unity.Mathematics;

namespace Core.Common.GameObject.Components
{
    public class TerrainLayer
    {
        public readonly string DiffuseMapPath;
        public readonly string NormalMapPath;
        public readonly float3 TileOffset;
        public readonly float3 TileSize;
        
        public TerrainLayer(string diffuseMapPath, string normalMapPath, float3 tileOffset, float3 tileSize)
        {
            DiffuseMapPath = diffuseMapPath;
            NormalMapPath = normalMapPath;
            TileOffset = tileOffset;
            TileSize = tileSize;
        }
    }
    
    public class Terrain: IUnprocessedComponent
    {
        public readonly int HeightmapResolution;
        public readonly int AlphamapResolution;
        public readonly float3 TerrainSize;
        public readonly float[,] Heightmap;
        public readonly float[,,] Alphamaps;
        public readonly TerrainLayer[] TerrainLayers;
        
        public Terrain(int heightmapResolution, int alphamapResolution, float3 terrainSize, float[,] heightmap,
            float[,,] alphamaps, TerrainLayer[] terrainLayers)
        {
            HeightmapResolution = heightmapResolution;
            AlphamapResolution = alphamapResolution;
            TerrainSize = terrainSize;
            Heightmap = heightmap;
            Alphamaps = alphamaps;
            TerrainLayers = terrainLayers;
        }
    }
}