using System;
using System.Collections.Generic;
using Core.MasterFile.Parser.Structures;

namespace Core.MasterFile.Parser.Extensions.Initialization
{
    public class CellExtensionInitializer : IMasterFileExtensionInitializer<CellExtensionData>
    {
        private const string WorldSpaceRecordType = "WRLD";
        private const string CellRecordType = "CELL";
        private const int ExteriorCellBlockGroupType = 4;
        private const int ExteriorCellSubBlockGroupType = 5;
        private const int WorldChildrenGroupType = 1;

        private readonly Dictionary<ExteriorCellSubBlockId, List<uint>> _exteriorCellSubBlockToCellFormIds = new();
        private readonly Dictionary<uint, long> _worldSpaceFormIdToPersistentCellPosition = new();
        private readonly Dictionary<uint, uint> _cellFormIDToWorldSpaceFormId = new();

        private uint _currentWorldSpaceFormId;
        private short _currentCellBlockX;
        private short _currentCellBlockY;
        private ExteriorCellSubBlockId? _currentExteriorCellSubBlockId;

        public void OnRecordHeaderParsed(long recordStartPosition, Record record, Group parentGroupHeader)
        {
            switch (record.Type)
            {
                case WorldSpaceRecordType:
                    _currentWorldSpaceFormId = record.FormId;
                    break;
                case CellRecordType
                    when parentGroupHeader is { GroupType: ExteriorCellSubBlockGroupType }
                         && _currentExteriorCellSubBlockId != null:
                    _exteriorCellSubBlockToCellFormIds[_currentExteriorCellSubBlockId.Value].Add(record.FormId);
                    _cellFormIDToWorldSpaceFormId[record.FormId] = _currentWorldSpaceFormId;
                    break;
                case CellRecordType
                    when parentGroupHeader is { GroupType: WorldChildrenGroupType }:
                    _worldSpaceFormIdToPersistentCellPosition[record.FormId] = recordStartPosition;
                    _cellFormIDToWorldSpaceFormId[record.FormId] = _currentWorldSpaceFormId;
                    break;
            }
        }

        public void OnGroupHeaderParsed(long groupStartPosition, Group group, Group parentGroupHeader)
        {
            switch (group.GroupType)
            {
                case ExteriorCellBlockGroupType:
                    _currentCellBlockY = BitConverter.ToInt16(new[] { group.Label[0], group.Label[1] }, 0);
                    _currentCellBlockX = BitConverter.ToInt16(new[] { group.Label[2], group.Label[3] }, 0);
                    break;
                case ExteriorCellSubBlockGroupType:
                    var currentCellSubBlockY =
                        BitConverter.ToInt16(new[] { group.Label[0], group.Label[1] }, 0);
                    var currentCellSubBlockX =
                        BitConverter.ToInt16(new[] { group.Label[2], group.Label[3] }, 0);
                    _currentExteriorCellSubBlockId = new ExteriorCellSubBlockId(_currentWorldSpaceFormId,
                        _currentCellBlockX, _currentCellBlockY, currentCellSubBlockX, currentCellSubBlockY);
                    _exteriorCellSubBlockToCellFormIds[_currentExteriorCellSubBlockId.Value] = new List<uint>();
                    break;
            }
        }

        public CellExtensionData InitializationResult =>
            new(_exteriorCellSubBlockToCellFormIds,
                _worldSpaceFormIdToPersistentCellPosition,
                _cellFormIDToWorldSpaceFormId);
    }
}