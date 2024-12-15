using Core.Common.GameObject;
using Core.Common.GameObject.Components.Mesh;
using Core.Common.PreloadApis;
using Core.Common.Structures;
using Core.MasterFile.Converter.Cell.Delegate.Reference.Base;
using Core.MasterFile.Manager;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Converter.Cell.Delegate.Reference
{
    public class StaticObjectDelegate : ICellReferenceDelegate
    {
        private readonly IMeshPreloader _meshPreloader;
        private readonly MasterFileManager _masterFileManager;

        public StaticObjectDelegate(IMeshPreloader meshPreloader, MasterFileManager masterFileManager)
        {
            _meshPreloader = meshPreloader;
            _masterFileManager = masterFileManager;
        }
        
        public bool IsApplicable(CELL cell, REFR reference, Record referencedRecord, CellInfoBuilder resultBuilder)
        {
            return referencedRecord is STAT or MSTT or FURN or TREE;
        }

        public void ProcessReference(CELL cell, REFR reference, Record referencedRecord, CellInfoBuilder resultBuilder)
        {
            var modelInfo = referencedRecord switch
            {
                STAT stat => stat.ModelInfo,
                MSTT mstt => mstt.ModelInfo,
                FURN furn => furn.ModelInfo,
                TREE tree => tree.ModelInfo,
                _ => null
            };
            if (modelInfo == null) return;

            var meshInfo = modelInfo.ToMeshInfo(_masterFileManager);
            _meshPreloader.PreloadMesh(meshInfo);
            
            var editorId = referencedRecord switch
            {
                STAT stat => stat.EditorID,
                MSTT mstt => mstt.EditorID,
                FURN furn => furn.EditorID,
                TREE tree => tree.EditorID,
                _ => null
            };
            var gameObj = new GameObject(editorId ?? referencedRecord.FormId.ToString(), resultBuilder.RootGameObject);
            CellUtils.ApplyPositionAndRotation(reference.Position.XYZ, reference.Rotation.XYZ, reference.Scale, gameObj);
            gameObj.Components.Add(new UnprocessedMeshComponent(meshInfo));
            resultBuilder.UnprocessedGameObjects.Add(gameObj);
        }
    }
}