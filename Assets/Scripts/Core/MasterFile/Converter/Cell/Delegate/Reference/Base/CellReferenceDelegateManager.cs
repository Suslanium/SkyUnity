using System.Collections.Generic;
using Core.Common.Structures;
using Core.MasterFile.Converter.Cell.Delegate.Base;
using Core.MasterFile.Manager.Extensions;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Converter.Cell.Delegate.Reference.Base
{
    public class CellReferenceDelegateManager : ICellRecordPreprocessDelegate<REFR>, ICellRecordInstantiationDelegate<REFR>
    {
        private readonly IReadOnlyList<ICellReferencePreprocessDelegate> _preprocessDelegates;
        private readonly IReadOnlyList<ICellReferenceInstantiationDelegate> _instantiationDelegates;
        
        public CellReferenceDelegateManager(IReadOnlyList<ICellReferencePreprocessDelegate> preprocessDelegates,
            IReadOnlyList<ICellReferenceInstantiationDelegate> instantiationDelegates)
        {
            _preprocessDelegates = preprocessDelegates;
            _instantiationDelegates = instantiationDelegates;
        }
        
        public void PreprocessRecord(RawCellData rawCellData, REFR record, CellInfo result)
        {
            if (!rawCellData.ReferenceBaseObjects.TryGetValue(record.BaseObjectFormId, out var referencedRecord))
            {
                return;
            }
            
            foreach (var preprocessDelegate in _preprocessDelegates)
            {
                if (!preprocessDelegate.IsPreprocessApplicable(rawCellData.CellRecord, record, referencedRecord))
                {
                    continue;
                }

                preprocessDelegate.PreprocessObject(rawCellData.CellRecord, record, referencedRecord, result);
            }
        }

        public void InstantiateRecord(RawCellData rawCellData, REFR record, CellInfo result)
        {
            if (!rawCellData.ReferenceBaseObjects.TryGetValue(record.BaseObjectFormId, out var referencedRecord))
            {
                return;
            }
            if (referencedRecord == null)
            {
                return;
            }

            foreach (var instantiationDelegate in _instantiationDelegates)
            {
                if (!instantiationDelegate.IsInstantiationApplicable(rawCellData.CellRecord, record, referencedRecord))
                    continue;

                instantiationDelegate.InstantiateObject(rawCellData.CellRecord, record, referencedRecord, result);
            }
        }
    }
}