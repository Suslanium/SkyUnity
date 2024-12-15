using Core.Common;
using Core.Common.Converter;
using Core.Common.Structures;
using Core.MasterFile.Converter.Cell.Delegate.Reference.Base;
using Core.MasterFile.Parser.Structures;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Converter.Cell.Delegate.Reference
{
    public class CocPositionDelegate : ICellReferenceDelegate
    {
        private const uint CocMarkerFormID = 0x32;
        private const uint IsMarkerFlagMask = 0x00800000;
        
        public bool IsApplicable(CELL cell, REFR reference, Record referencedRecord, CellInfoBuilder resultBuilder)
        {
            if (referencedRecord is STAT { FormId: CocMarkerFormID })
            {
                return true;
            }
            if (resultBuilder.DefaultSpawnPosition == null && resultBuilder.DefaultSpawnRotation == null &&
                Utils.IsFlagSet(referencedRecord.Flag, IsMarkerFlagMask))
            {
                return true;
            }

            return false;
        }

        public void ProcessReference(CELL cell, REFR reference, Record referencedRecord, CellInfoBuilder resultBuilder)
        {
            TransformConverter.SkyrimPointToUnityPoint(reference.Position.XYZ, out var position);
            TransformConverter.SkyrimRadiansToUnityQuaternion(reference.Rotation.XYZ, out var rotation);
            resultBuilder.DefaultSpawnPosition = position;
            resultBuilder.DefaultSpawnRotation = rotation;
        }
    }
}