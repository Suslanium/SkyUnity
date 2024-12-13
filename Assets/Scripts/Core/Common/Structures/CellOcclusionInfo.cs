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

        public CellOcclusionInfo(IReadOnlyDictionary<uint, GameObject.GameObject> rooms,
            IReadOnlyList<(GameObject.GameObject, uint, uint)> portals, IReadOnlyList<(uint, uint)> roomConnections)
        {
            Rooms = rooms;
            Portals = portals;
            RoomConnections = roomConnections;
        }
    }
}