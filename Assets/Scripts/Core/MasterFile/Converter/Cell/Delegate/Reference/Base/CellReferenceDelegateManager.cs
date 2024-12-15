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
        
        public void ProcessRecord(RawCellData rawCellData, REFR record, CellInfoBuilder resultBuilder)
        {
            if (!rawCellData.ReferenceBaseObjects.TryGetValue(record.BaseObjectFormId, out var referencedRecord))
            {
                return;
            }
            
            foreach (var processDelegate in _delegates)
            {
                if (!processDelegate.IsApplicable(rawCellData.CellRecord, record, referencedRecord, resultBuilder))
                {
                    continue;
                }

                processDelegate.ProcessReference(rawCellData.CellRecord, record, referencedRecord, resultBuilder);
            }
        }
    }
}