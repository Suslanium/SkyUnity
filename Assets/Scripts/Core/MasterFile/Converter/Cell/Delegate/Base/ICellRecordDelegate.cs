using Core.Common.Structures;
using Core.MasterFile.Manager.Extensions;
using Core.MasterFile.Parser.Structures;

namespace Core.MasterFile.Converter.Cell.Delegate.Base
{
    public interface ICellRecordDelegate<T> where T : Record
    {
        public void ProcessRecord(RawCellData rawCellData, T record, CellInfo result);
    }
}