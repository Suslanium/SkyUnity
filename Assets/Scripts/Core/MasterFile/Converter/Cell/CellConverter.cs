using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Common.GameObject;
using Core.Common.Structures;
using Core.MasterFile.Converter.Cell.Delegate.Base;
using Core.MasterFile.Manager.Extensions;
using Core.MasterFile.Parser.Structures;

namespace Core.MasterFile.Converter.Cell
{
    public class CellConverter
    {
        private readonly IReadOnlyCollection<ICellDelegate> _cellDelegates;
        private readonly IReadOnlyCollection<ICellRecordDelegate> _cellRecordDelegates;

        public CellConverter(IReadOnlyCollection<ICellDelegate> cellDelegates,
            IReadOnlyCollection<ICellRecordDelegate> cellRecordDelegates)
        {
            _cellDelegates = cellDelegates;
            _cellRecordDelegates = cellRecordDelegates;
        }

        public CellInfo ConvertRawCellData(RawCellData rawCell, bool persistentOnly = false)
        {
            var rootCellObject = new GameObject(rawCell.CellRecord.EditorID ?? rawCell.CellRecord.FormId.ToString());
            var builder = new CellInfoBuilder(rootCellObject);
            foreach (var cellDelegate in _cellDelegates)
            {
                cellDelegate.ProcessCell(rawCell, builder);
            }

            var concurrentlyProcessedRecords = new List<Task>();
            foreach (var persistentRecord in rawCell.PersistentChildren)
            {
                foreach (var cellRecordDelegate in _cellRecordDelegates)
                {
                    TryProcessRecord(rawCell, builder, persistentRecord, cellRecordDelegate,
                        concurrentlyProcessedRecords);
                }
            }

            if (!persistentOnly)
            {
                foreach (var temporaryRecord in rawCell.TemporaryChildren)
                {
                    foreach (var cellRecordDelegate in _cellRecordDelegates)
                    {
                        TryProcessRecord(rawCell, builder, temporaryRecord, cellRecordDelegate,
                            concurrentlyProcessedRecords);
                    }
                }
            }

            Task.WaitAll(concurrentlyProcessedRecords.ToArray());
            return builder.Build();
        }

        private static void TryProcessRecord(
            RawCellData rawCell,
            CellInfoBuilder builder,
            Record record,
            ICellRecordDelegate recordDelegate,
            List<Task> concurrentlyProcessedRecords)
        {
            if (!recordDelegate.IsApplicable(record))
            {
                return;
            }

            if (recordDelegate.IsConcurrent)
            {
                var task = Task.Run(() => { recordDelegate.ProcessRecord(rawCell, record, builder); });
                concurrentlyProcessedRecords.Add(task);
            }
            else
            {
                recordDelegate.ProcessRecord(rawCell, record, builder);
            }
        }
    }
}