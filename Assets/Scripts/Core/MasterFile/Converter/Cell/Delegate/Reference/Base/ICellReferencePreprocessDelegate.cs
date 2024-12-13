using Core.Common.Structures;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Converter.Cell.Delegate.Reference.Base
{
    public interface ICellReferencePreprocessDelegate
    {
        public bool IsPreprocessApplicable(CELL cell, REFR reference, Record referencedRecord);

        public void PreprocessObject(CELL cell, REFR reference, Record referencedRecord, CellInfo result);
    }
}