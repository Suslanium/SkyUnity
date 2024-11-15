using Core.MasterFile.Parser.Structures.Records.FieldStructures.General;

namespace Core.MasterFile.Parser.Structures.Records.FieldStructures
{
    public class DoorTeleport
    {
        /// <summary>
        /// The door reference in the destination cell.
        /// </summary>
        public readonly uint DestinationDoorReference;

        /// <summary>
        /// Position where the player should spawn after teleporting.
        /// </summary>
        public readonly Float32Vector3 DestinationPosition;

        /// <summary>
        /// Rotation which the player should be facing after teleporting.
        /// </summary>
        public readonly Float32Vector3 DestinationRotation;

        /// <summary>
        /// 0x01 - No alarm
        /// </summary>
        public readonly uint Flag;

        public DoorTeleport(uint destinationDoorReference, Float32Vector3 destinationPosition,
            Float32Vector3 destinationRotation, uint flag)
        {
            DestinationDoorReference = destinationDoorReference;
            DestinationPosition = destinationPosition;
            DestinationRotation = destinationRotation;
            Flag = flag;
        }
    }
}