using Core.Common.Structures;
using Core.MasterFile.Manager.Extensions;
using Core.MasterFile.Parser.Structures;

namespace Core.MasterFile.Converter.Cell.Delegate.Base
{
    public interface ICellRecordInstantiationDelegate<T> where T : Record
    {
        public void InstantiateRecord(RawCellData rawCellData, T record, CellInfo result);
    }
}