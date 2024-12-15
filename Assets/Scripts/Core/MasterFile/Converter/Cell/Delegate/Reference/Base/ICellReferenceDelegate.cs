using Core.Common.Structures;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Converter.Cell.Delegate.Reference.Base
{
    public interface ICellReferenceDelegate
    {
        public bool IsApplicable(CELL cell, REFR reference, Record referencedRecord, CellInfoBuilder resultBuilder);

        public void ProcessReference(CELL cell, REFR reference, Record referencedRecord, CellInfoBuilder resultBuilder);
    }
}