using Core.Common.Structures;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Converter.Cell.Delegate.Reference.Base
{
    public interface ICellReferenceInstantiationDelegate
    {
        public bool IsInstantiationApplicable(CELL cell, REFR reference, Record referencedRecord);

        public void InstantiateObject(CELL cell, REFR reference, Record referencedRecord, CellInfo result);
    }
}