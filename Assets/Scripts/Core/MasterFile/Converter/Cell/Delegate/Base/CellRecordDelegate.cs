using Core.Common.Structures;
using Core.MasterFile.Manager.Extensions;
using Core.MasterFile.Parser.Structures;

namespace Core.MasterFile.Converter.Cell.Delegate.Base
{
    public abstract class CellRecordDelegate<T> : ICellRecordDelegate where T : Record
    {
        protected abstract void ProcessRecord(RawCellData rawCellData, T record, CellInfoBuilder resultBuilder);

        public bool IsApplicable(Record record)
        {
            return record is T;
        }

        void ICellRecordDelegate.ProcessRecord(RawCellData rawCellData, Record record, CellInfoBuilder resultBuilder)
        {
            if (record is not T typedRecord)
            {
                // TODO Logging?
                return;
            }
            ProcessRecord(rawCellData,  typedRecord, resultBuilder);
        }
    }

    public interface ICellRecordDelegate
    {
        //Set this to true ONLY for delegates that take a long time to process, potentially halting the processing of the cell
        public bool IsConcurrent => false;

        public bool IsApplicable(Record record);

        public void ProcessRecord(RawCellData rawCellData, Record record, CellInfoBuilder resultBuilder);
    }
}