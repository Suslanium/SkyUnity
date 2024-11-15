namespace Core.MasterFile.Parser.Structures.Records.FieldStructures
{
    /// <summary>
    /// Portals are used by occlusion culling to connect two rooms together.
    /// Not to confuse with teleportation/door portals.
    /// </summary>
    public class PortalInfo
    {
        /// <summary>
        /// Origin room REFR formID
        /// </summary>
        public readonly uint OriginReference;
        
        /// <summary>
        /// Destination room REFR formID
        /// </summary>
        public readonly uint DestinationReference;

        public PortalInfo(uint originReference, uint destinationReference)
        {
            OriginReference = originReference;
            DestinationReference = destinationReference;
        }
    }
}