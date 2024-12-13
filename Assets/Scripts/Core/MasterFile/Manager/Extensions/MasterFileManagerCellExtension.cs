using System.Collections.Generic;
using System.Linq;
using Core.MasterFile.Common.Structures;
using Core.MasterFile.Parser.Extensions;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Manager.Extensions
{
    public class RawCellData
    {
        /// <summary>
        /// Persistent cell children, mostly containing REFR records.
        /// </summary>
        public readonly IReadOnlyList<Record> PersistentChildren;

        /// <summary>
        /// Temporary cell children, mostly containing REFR records.
        /// </summary>
        public readonly IReadOnlyList<Record> TemporaryChildren;

        /// <summary>
        /// Base objects for references (REFR records) in the cell.
        /// </summary>
        public readonly IReadOnlyDictionary<uint, Record> ReferenceBaseObjects;

        /// <summary>
        /// Cell record from the last master file that contains it.
        /// (Last here is referred to the load order of the master files)
        /// </summary>
        public readonly CELL CellRecord;

        public RawCellData(IReadOnlyList<Record> persistentChildren, IReadOnlyList<Record> temporaryChildren,
            IReadOnlyDictionary<uint, Record> referenceBaseObjects, CELL cellRecord)
        {
            PersistentChildren = persistentChildren;
            TemporaryChildren = temporaryChildren;
            ReferenceBaseObjects = referenceBaseObjects;
            CellRecord = cellRecord;
        }
    }

    public class CellLoadRequest
    {
        /// <summary>
        /// All the master files that contain the cell, ordered by the load order.
        /// </summary>
        public readonly IReadOnlyList<(Parser.MasterFile, CELL)> MasterFileToCellInLoadOrder;

        /// <summary>
        /// Cell record from the last master file that contains it.
        /// (Last here is referred to the load order of the master files)
        /// </summary>
        public readonly CELL CellRecord;

        public CellLoadRequest(IReadOnlyList<(Parser.MasterFile, CELL)> masterFileToCellInLoadOrder, CELL cellRecord)
        {
            MasterFileToCellInLoadOrder = masterFileToCellInLoadOrder;
            CellRecord = cellRecord;
        }
    }

    public class MasterFileManagerCellExtension : MasterFileManagerExtension
    {
        private const int CellChildrenGroupType = 6;
        private const int CellPersistentChildrenSubgroupType = 8;
        private const int CellTemporaryChildrenSubgroupType = 9;

        public MasterFileManagerCellExtension(MasterFileManager masterFileManager) : base(masterFileManager) {}

        public CELL FindCellByEditorId(string editorId)
        {
            MasterFileManager.MasterFilesInitialization.Wait();

            return MasterFileManager.ReverseLoadOrder
                .Select(fileName => MasterFileManager.MasterFiles[fileName].FindCellByEditorId(editorId))
                .FirstOrDefault(cell => cell != null);
        }

        public uint GetWorldSpaceFormId(uint cellFormId)
        {
            MasterFileManager.MasterFilesInitialization.Wait();

            var masterFileName = MasterFileManager.ReverseLoadOrder.FirstOrDefault(fileName =>
                MasterFileManager.MasterFiles[fileName].RecordExists(cellFormId));

            return masterFileName == null
                ? 0
                : MasterFileManager.MasterFiles[masterFileName].GetWorldSpaceFormId(cellFormId);
        }

        public RawCellData GetWorldSpacePersistentCellData(uint worldSpaceFormId)
        {
            MasterFileManager.MasterFilesInitialization.Wait();

            List<(Parser.MasterFile, CELL)> masterFileToCellInLoadOrder = new();
            CELL cellRecord = null;

            foreach (var masterFile in MasterFileManager.ReverseLoadOrder
                         .Select(fileName => MasterFileManager.MasterFiles[fileName]).Reverse())
            {
                var currentCellRecord = masterFile.GetPersistentWorldSpaceCell(worldSpaceFormId);
                if (currentCellRecord == null) continue;
                
                masterFileToCellInLoadOrder.Add((masterFile, currentCellRecord));
                cellRecord = currentCellRecord;
            }
            
            return GetCellData(new CellLoadRequest(masterFileToCellInLoadOrder, cellRecord));
        }

        public RawCellData GetCellData(uint cellFormId)
        {
            MasterFileManager.MasterFilesInitialization.Wait();
            
            List<(Parser.MasterFile, CELL)> masterFileToCellInLoadOrder = new();
            CELL cellRecord = null;
            
            foreach (var masterFile in MasterFileManager.ReverseLoadOrder
                         .Select(fileName => MasterFileManager.MasterFiles[fileName]).Reverse())
            {
                var currentCellRecord = masterFile.GetFromFormId<CELL>(cellFormId);
                if (currentCellRecord == null) continue;
                
                masterFileToCellInLoadOrder.Add((masterFile, currentCellRecord));
                cellRecord = currentCellRecord;
            }
            
            return GetCellData(new CellLoadRequest(masterFileToCellInLoadOrder, cellRecord));
        }

        public RawCellData GetExteriorCellData(FullCellPosition cellPosition)
        {
            MasterFileManager.MasterFilesInitialization.Wait();
            
            List<(Parser.MasterFile, CELL)> masterFileToCellInLoadOrder = new();
            CELL cellRecord = null;

            foreach (var masterFile in MasterFileManager.ReverseLoadOrder
                         .Select(fileName => MasterFileManager.MasterFiles[fileName]).Reverse())
            {
                var currentCellRecord = masterFile.GetExteriorCellByPosition(cellPosition);
                if (currentCellRecord == null) continue;
                
                masterFileToCellInLoadOrder.Add((masterFile, currentCellRecord));
                cellRecord = currentCellRecord;
            }
            
            return GetCellData(new CellLoadRequest(masterFileToCellInLoadOrder, cellRecord));
        }

        private static RawCellData GetCellData(CellLoadRequest request)
        {
            var persistentChildren = new Dictionary<uint, Record>();
            var temporaryChildren = new Dictionary<uint, Record>();
            var baseObjects = new Dictionary<uint, Record>();

            foreach (var (masterFile, cellRecord) in request.MasterFileToCellInLoadOrder)
            {
                var cellChildrenGroup = masterFile.ReadAfterRecord(cellRecord);

                if (cellChildrenGroup is not Group { GroupType: CellChildrenGroupType } cellChildren)
                {
                    //TODO log warning
                    continue;
                }

                foreach (var subGroup in cellChildren.GroupData)
                {
                    if (subGroup is not Group group) continue;
                    if (group.GroupType != CellPersistentChildrenSubgroupType &&
                        group.GroupType != CellTemporaryChildrenSubgroupType) continue;

                    foreach (var entry in group.GroupData)
                    {
                        if (entry is not Record record) continue;
                        if (group.GroupType == CellPersistentChildrenSubgroupType)
                        {
                            persistentChildren[record.FormId] = record;
                        }
                        else
                        {
                            temporaryChildren[record.FormId] = record;
                        }

                        if (record is REFR reference && reference.BaseObjectFormId != 0 &&
                            !baseObjects.ContainsKey(reference.BaseObjectFormId))
                        {
                            baseObjects[reference.BaseObjectFormId] =
                                masterFile.GetFromFormId<Record>(reference.BaseObjectFormId);
                        }
                    }
                }
            }

            return new RawCellData(persistentChildren.Values.ToList(), temporaryChildren.Values.ToList(), baseObjects,
                request.CellRecord);
        }
    }
}