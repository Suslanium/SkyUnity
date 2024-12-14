using Core.Common.GameObject.Components.Localization;
using Unity.Mathematics;

namespace Core.Common.GameObject.Components
{
    public class DoorTeleport : IComponent
    {
        public readonly bool IsAutomatic;
        public readonly float3 DestinationPosition;
        public readonly quaternion DestinationRotation;
        public readonly uint DestinationCellFormId;
        public readonly string DestinationName;

        public DoorTeleport(bool isAutomatic, float3 destinationPosition, quaternion destinationRotation,
            uint destinationCellFormId, string destinationName)
        {
            IsAutomatic = isAutomatic;
            DestinationPosition = destinationPosition;
            DestinationRotation = destinationRotation;
            DestinationCellFormId = destinationCellFormId;
            DestinationName = destinationName;
        }
    }

    public class DoorTeleportNonLocalized : ILocalizableComponent
    {
        private readonly bool _isAutomatic;
        private readonly float3 _destinationPosition;
        private readonly quaternion _destinationRotation;
        private readonly uint _destinationCellFormId;
        private readonly LocalizedStringInfo _destinationName;
        public LocalizedStringInfo LocalizedStringInfo => _destinationName;
        
        public DoorTeleportNonLocalized(bool isAutomatic, float3 destinationPosition, quaternion destinationRotation,
            uint destinationCellFormId, LocalizedStringInfo destinationName)
        {
            _isAutomatic = isAutomatic;
            _destinationPosition = destinationPosition;
            _destinationRotation = destinationRotation;
            _destinationCellFormId = destinationCellFormId;
            _destinationName = destinationName;
        }
        
        public IComponent GetLocalizedComponent(string resolvedLocalizedString)
        {
            return new DoorTeleport(_isAutomatic, _destinationPosition, _destinationRotation, _destinationCellFormId,
                resolvedLocalizedString);
        }
    }
}