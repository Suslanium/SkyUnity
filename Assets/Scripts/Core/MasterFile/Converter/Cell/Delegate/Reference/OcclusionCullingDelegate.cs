using Core.Common.Converter;
using Core.Common.GameObject;
using Core.Common.GameObject.Components;
using Core.Common.Structures;
using Core.MasterFile.Converter.Cell.Delegate.Reference.Base;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Converter.Cell.Delegate.Reference
{
    public class OcclusionCullingDelegate : ICellReferenceDelegate
    {
        private const uint PortalFormId = 0x20;
        private const uint RoomFormId = 0x1F;

        public bool IsApplicable(CELL cell, REFR reference, Record referencedRecord, CellInfoBuilder resultBuilder)
        {
            return referencedRecord is STAT { FormId: PortalFormId or RoomFormId };
        }

        public void ProcessReference(CELL cell, REFR reference, Record referencedRecord, CellInfoBuilder resultBuilder)
        {
            if (referencedRecord is not STAT stat) return;

            if (stat.FormId == PortalFormId)
            {
                var gameObj = new GameObject("Portal marker");
                CellUtils.ApplyPositionAndRotation(reference.Position.XYZ, reference.Rotation.XYZ, reference.Scale, gameObj);

                TransformConverter.SkyrimPointToUnityPoint(reference.Primitive.Bounds.XYZ, out var colliderSize);
                colliderSize *= 2;
                var collider = new BoxCollider(true, colliderSize);
                gameObj.Components.Add(collider);

                if (reference.PortalDestinations == null) return;
                gameObj.Parent = resultBuilder.RootGameObject;
                resultBuilder.OcclusionInfoBuilder.Portals.Add((gameObj,
                    reference.PortalDestinations.OriginReference,
                    reference.PortalDestinations.DestinationReference));
            }
            else if (stat.FormId == RoomFormId)
            {
                var gameObj = new GameObject("Room marker", resultBuilder.RootGameObject);
                CellUtils.ApplyPositionAndRotation(reference.Position.XYZ, reference.Rotation.XYZ, reference.Scale, gameObj);
                
                TransformConverter.SkyrimPointToUnityPoint(reference.Primitive.Bounds.XYZ, out var colliderSize);
                colliderSize *= 2;
                var collider = new BoxCollider(true, colliderSize);
                gameObj.Components.Add(collider);
                
                resultBuilder.OcclusionInfoBuilder.Rooms.Add(reference.FormId, gameObj);
                if (reference.LinkedRoomFormIds.Count <= 0) return;
                foreach (var linkedRoomFormId in reference.LinkedRoomFormIds)
                {
                    resultBuilder.OcclusionInfoBuilder.RoomConnections.Add((reference.FormId, linkedRoomFormId));
                }
            }
        }
    }
}