using System.Collections.Generic;
using Core.Common.Structures;
using Core.MasterFile.Converter.Cell.Delegate.Base;
using Core.MasterFile.Manager.Extensions;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Converter.Cell.Delegate.Reference.Base
{
    public class CellReferenceDelegateManager : ICellRecordDelegate<REFR>
    {
        private readonly IReadOnlyList<ICellReferenceDelegate> _delegates;
        
        public CellReferenceDelegateManager(IReadOnlyList<ICellReferenceDelegate> delegates)
        {
            _delegates = delegates;
        }
        
        public void ProcessRecord(RawCellData rawCellData, REFR record, CellInfo result)
        {
            if (!rawCellData.ReferenceBaseObjects.TryGetValue(record.BaseObjectFormId, out var referencedRecord))
            {
                return;
            }
            
            foreach (var preprocessDelegate in _delegates)
            {
                if (!preprocessDelegate.IsApplicable(rawCellData.CellRecord, record, referencedRecord))
                {
                    continue;
                }

                preprocessDelegate.ProcessReference(rawCellData.CellRecord, record, referencedRecord, result);
            }
        }
    }
}