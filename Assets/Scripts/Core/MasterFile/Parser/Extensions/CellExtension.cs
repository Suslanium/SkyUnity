using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core.MasterFile.Parser.Extensions.Initialization;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser.Extensions
{
    public readonly struct ExteriorCellSubBlockId : IEquatable<ExteriorCellSubBlockId>
    {
        public readonly uint WorldSpaceFormID;

        public readonly short BlockX;
        public readonly short BlockY;

        public readonly short SubBlockX;
        public readonly short SubBlockY;

        public ExteriorCellSubBlockId(
            uint worldSpaceFormID,
            short blockX,
            short blockY,
            short subBlockX,
            short subBlockY)
        {
            WorldSpaceFormID = worldSpaceFormID;
            BlockX = blockX;
            BlockY = blockY;
            SubBlockX = subBlockX;
            SubBlockY = subBlockY;
        }

        //Everything below in this struct is auto-generated
        public bool Equals(ExteriorCellSubBlockId other)
        {
            return WorldSpaceFormID == other.WorldSpaceFormID && BlockX == other.BlockX && BlockY == other.BlockY &&
                   SubBlockX == other.SubBlockX && SubBlockY == other.SubBlockY;
        }

        public override bool Equals(object obj)
        {
            return obj is ExteriorCellSubBlockId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(WorldSpaceFormID, BlockX, BlockY, SubBlockX, SubBlockY);
        }
    }

    public readonly struct CellPosition : IEquatable<CellPosition>
    {
        public readonly int X;
        public readonly int Y;

        public CellPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        //Everything below in this struct is auto-generated
        public bool Equals(CellPosition other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is CellPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }

    public class CellExtensionData
    {
        public readonly IReadOnlyDictionary<ExteriorCellSubBlockId, List<uint>> ExteriorCellSubBlockToCellFormIds;
        public readonly IReadOnlyDictionary<uint, long> WorldSpaceFormIdToPersistentCellPosition;
        public readonly IReadOnlyDictionary<uint, uint> CellFormIDToWorldSpaceFormId;

        public readonly ConcurrentDictionary<ExteriorCellSubBlockId, Dictionary<CellPosition, CELL>>
            LoadedExteriorCellSubBlocks = new();

        public CellExtensionData(
            IReadOnlyDictionary<ExteriorCellSubBlockId, List<uint>> exteriorCellSubBlockToCellFormIds,
            IReadOnlyDictionary<uint, long> worldSpaceFormIdToPersistentCellPosition,
            IReadOnlyDictionary<uint, uint> cellFormIDToWorldSpaceFormId)
        {
            ExteriorCellSubBlockToCellFormIds = exteriorCellSubBlockToCellFormIds;
            WorldSpaceFormIdToPersistentCellPosition = worldSpaceFormIdToPersistentCellPosition;
            CellFormIDToWorldSpaceFormId = cellFormIDToWorldSpaceFormId;
        }
    }

    public class CellExtension : MasterFileExtension<CellExtensionData>
    {
        private const string CellRecordType = "CELL";

        public CellExtension(CellExtensionInitializer initializer) : base(initializer) {}

        public CELL FindCellByEditorId(string editorId)
        {
            MasterFile.EnsureInitialized();

            var cellRecordDictionary = MasterFile.RecordTypeToFormIdToPosition[CellRecordType];
            return cellRecordDictionary.Keys.Select(formId => MasterFile.GetFromFormId<CELL>(formId))
                .FirstOrDefault(record => record?.EditorID == editorId);
        }

        public uint GetWorldSpaceFormId(uint cellFormId)
        {
            MasterFile.EnsureInitialized();
            
            ExtensionData.CellFormIDToWorldSpaceFormId.TryGetValue(cellFormId, out var worldSpaceFormId);
            return worldSpaceFormId;
        }

        private Dictionary<CellPosition, CELL> LoadCellSubBlock(ExteriorCellSubBlockId exteriorCellSubBlockId)
        {
            MasterFile.EnsureInitialized();
            
            if (ExtensionData.LoadedExteriorCellSubBlocks.TryGetValue(exteriorCellSubBlockId, out var loadedSubBlock))
            {
                return loadedSubBlock;
            }

            if (!ExtensionData.ExteriorCellSubBlockToCellFormIds.TryGetValue(exteriorCellSubBlockId, out var cellFormIds))
            {
                return null;
            }

            var cellDictionary = new Dictionary<CellPosition, CELL>();
            foreach (var cellFormId in cellFormIds)
            {
                var cell = MasterFile.GetFromFormId<CELL>(cellFormId);
                if (cell == null) continue;
                cellDictionary.Add(new CellPosition(cell.XGridPosition, cell.YGridPosition), cell);
            }

            ExtensionData.LoadedExteriorCellSubBlocks.TryAdd(exteriorCellSubBlockId, cellDictionary);
            return cellDictionary;
        }

        public CELL GetExteriorCellByGridPosition(uint worldSpaceFormId, short blockX, short blockY, short subBlockX,
            short subBlockY, int xGridPosition, int yGridPosition)
        {
            MasterFile.EnsureInitialized();
            
            var exteriorCellSubBlockId =
                new ExteriorCellSubBlockId(worldSpaceFormId, blockX, blockY, subBlockX, subBlockY);
            var cellDictionary = LoadCellSubBlock(exteriorCellSubBlockId);
            if (cellDictionary == null)
            {
                return null;
            }

            cellDictionary.TryGetValue(new CellPosition(xGridPosition, yGridPosition), out var cell);
            return cell;
        }

        public CELL GetPersistentWorldSpaceCell(uint worldSpaceFormId)
        {
            MasterFile.EnsureInitialized();
            
            if (!ExtensionData.WorldSpaceFormIdToPersistentCellPosition.TryGetValue(worldSpaceFormId, out var position))
            {
                return null;
            }

            lock (FileReader)
            {
                var entry = MasterFileReader.ReadEntry(MasterFile.Properties, FileReader, position);
                return entry as CELL;
            }
        }
    }
}