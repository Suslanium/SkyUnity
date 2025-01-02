using Core.Common.Structures;
using Core.MasterFile.Manager.Extensions;

namespace Core.MasterFile.Converter.Cell.Delegate.Base
{
    public interface ICellDelegate
    {
        public void ProcessCell(RawCellData rawCellData, CellInfoBuilder resultBuilder);
    }
}