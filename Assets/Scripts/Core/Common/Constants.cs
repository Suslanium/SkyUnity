namespace Core.Common
{
    public static class Constants
    {
        private const int YardInSkyrimUnits = 64;
        private const float MeterInYards = 1.09361f;
        public const float MeterInSkyrimUnits = MeterInYards * YardInSkyrimUnits;

        public const int SkyrimExteriorCellSideLengthInSamples = 33;
        public const int SkyrimExteriorCellSideLengthInSkyrimUnits = 4096;
        public const float SkyrimExteriorCellSideLengthInMeters =
            SkyrimExteriorCellSideLengthInSkyrimUnits / MeterInSkyrimUnits;
        public const int SkyrimExteriorCellQuadrantSideLengthInSamples = 17;
    }
}