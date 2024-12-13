using Core.Common.Structures;
using Core.MasterFile.Manager.Extensions;
using Core.MasterFile.Parser.Structures;

namespace Core.MasterFile.Converter.Cell.Delegate.Base
{
    /// <summary>
    /// Used to preprocess records. For example, this can be used to start 3d model loading in background.
    /// </summary>
    public interface ICellRecordPreprocessDelegate<T> where T : Record
    {
        public void PreprocessRecord(RawCellData rawCellData, T record, CellInfo result);
    }
}