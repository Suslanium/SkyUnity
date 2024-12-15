using System;
using Core.Common;
using Core.Common.Converter;
using Core.Common.GameObject.Components;
using Core.Common.GameObject.Components.Mesh;
using Core.Common.PreloadApis;
using Core.Common.Structures;
using Core.MasterFile.Converter.Cell.Delegate.Reference.Base;
using Core.MasterFile.Manager;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;
using Unity.Mathematics;
using UnityEngine;
using BoxCollider = Core.Common.GameObject.Components.BoxCollider;
using GameObject = Core.Common.GameObject.GameObject;
using Random = System.Random;

namespace Core.MasterFile.Converter.Cell.Delegate.Reference
{
    public class DoorDelegate : ICellReferenceDelegate
    {
        private readonly IMeshPreloader _meshPreloader;
        private readonly MasterFileManager _masterFileManager;
        private readonly Random _random = new(DateTime.Now.Millisecond);

        private const ushort IsInteriorFlagMask = 0x0001;
        private const byte IsAutomaticFlagMask = 0x02;

        public DoorDelegate(IMeshPreloader meshPreloader, MasterFileManager masterFileManager)
        {
            _meshPreloader = meshPreloader;
            _masterFileManager = masterFileManager;
        }

        public bool IsApplicable(CELL cell, REFR reference, Record referencedRecord, CellInfoBuilder resultBuilder)
        {
            //Only teleport doors are loaded because regular doors
            //will block the location without the ability to open them
            return referencedRecord is DOOR && reference.DoorTeleport != null;
        }

        public void ProcessReference(CELL cell, REFR reference, Record referencedRecord, CellInfoBuilder resultBuilder)
        {
            if (referencedRecord is not DOOR door) return;
            var gameObj = new GameObject(door.EditorID ?? door.FormId.ToString(), resultBuilder.RootGameObject);
            var isAddedToUnprocessed = false;

            //Try preloading the door model
            var modelInfo = door.ModelInfo;
            if (modelInfo != null)
            {
                var meshInfo = modelInfo.ToMeshInfo(_masterFileManager);
                _meshPreloader.PreloadMesh(meshInfo);
                gameObj.Components.Add(new UnprocessedMeshComponent(meshInfo));
                resultBuilder.UnprocessedGameObjects.Add(gameObj);
                isAddedToUnprocessed = true;
            }

            //Get the door bounds, create a trigger collider
            var doorBounds = new Bounds();
            var boundsA = new float3(door.Bounds.X1, door.Bounds.Y1, door.Bounds.Z1);
            var boundsB = new float3(door.Bounds.X2, door.Bounds.Y2, door.Bounds.Z2);
            doorBounds.SetMinMax(boundsA, boundsB);
            var collider = new BoxCollider(true, doorBounds.center, doorBounds.size);
            gameObj.Components.Add(collider);

            //Find the destination cell, otherwise return
            var destinationDoorId = door.RandomTeleports.Count > 0
                ? door.RandomTeleports[_random.Next(0, door.RandomTeleports.Count - 1)]
                : reference.DoorTeleport.DestinationDoorReference;
            var destinationCellFormId = _masterFileManager.GetRecordParentFormId(destinationDoorId);
            var destinationCell = _masterFileManager.GetFromFormId<CELL>(destinationCellFormId);
            if (destinationCell == null) return;

            var destinationName = destinationCell.Name.ToLocalizedStringInfo(destinationCell.EditorID);
            //If the door leads to an exterior cell, load the worldspace record to get the worldspace name
            if (!Utils.IsFlagSet(destinationCell.CellFlag, IsInteriorFlagMask))
            {
                var worldSpaceFormId = _masterFileManager.GetRecordParentFormId(destinationCell.FormId);
                var worldSpace = _masterFileManager.GetFromFormId<WRLD>(worldSpaceFormId);
                if (worldSpace != null)
                {
                    destinationName = worldSpace.InGameName.ToLocalizedStringInfo(worldSpace.EditorID);
                }
            }

            //Get the teleport destination position and rotation and check if the teleport is automatic
            TransformConverter.SkyrimPointToUnityPoint(reference.DoorTeleport.DestinationPosition.XYZ, 
                out var teleportPosition);
            TransformConverter.SkyrimRadiansToUnityQuaternion(reference.DoorTeleport.DestinationRotation.XYZ,
                out var teleportRotation);
            var isAutomaticDoor = Utils.IsFlagSet(door.Flags, IsAutomaticFlagMask);

            //Create the door component, create the game object
            var doorComponent = new DoorTeleportNonLocalized(isAutomaticDoor, teleportPosition, teleportRotation, destinationCellFormId,
                destinationName);
            gameObj.Components.Add(doorComponent);
            
            if (!isAddedToUnprocessed)
            {
                resultBuilder.UnprocessedGameObjects.Add(gameObj);
            }

            //Apply the position and rotation to the door game object
            CellUtils.ApplyPositionAndRotation(reference.Position.XYZ, reference.Rotation.XYZ, reference.Scale, gameObj);
        }
    }
}