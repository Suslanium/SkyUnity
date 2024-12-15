using System.Collections.Generic;

namespace Core.Common.Structures
{
    public class CellOcclusionInfo
    {
        // Uint is the room Id (FormId)
        public readonly IReadOnlyDictionary<uint, GameObject.GameObject> Rooms;
        // Uints are the room Ids (FormIds)
        public readonly IReadOnlyList<(GameObject.GameObject, uint, uint)> Portals;
        // Uints are the room Ids (FormIds)
        public readonly IReadOnlyList<(uint, uint)> RoomConnections;

        public CellOcclusionInfo(CellOcclusionInfoBuilder builder)
        {
            Rooms = builder.Rooms;
            Portals = builder.Portals;
            RoomConnections = builder.RoomConnections;
        }
    }
    
    public class CellOcclusionInfoBuilder
    {
        public readonly Dictionary<uint, GameObject.GameObject> Rooms = new();
        public readonly List<(GameObject.GameObject, uint, uint)> Portals = new();
        public readonly List<(uint, uint)> RoomConnections = new();
        
        public CellOcclusionInfo Build()
        {
            return new CellOcclusionInfo(this);
        }
    }
}