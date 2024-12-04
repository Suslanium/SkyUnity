using Core.MasterFile.Parser.Reader;
using Core.MasterFile.Parser.Structures.Records;

namespace Core.MasterFile.Parser.Structures
{
    /// <summary>
    /// This class is used to store general information
    /// about a master file that can be needed during parsing.
    /// (e.g. the IsLocalized property is used to determine how to parse lstrings)
    /// </summary>
    public class MasterFileProperties
    {
        public readonly bool IsMaster;
        public readonly bool IsLocalized;
        public readonly bool IsLight;

        private MasterFileProperties(bool isMaster, bool isLocalized, bool isLight)
        {
            IsMaster = isMaster;
            IsLocalized = isLocalized;
            IsLight = isLight;
        }

        public static MasterFileProperties DummyInstance => new(false, false, false);

        // ReSharper disable once InconsistentNaming
        public static MasterFileProperties FromTES4(TES4 tes4)
        {
            return new MasterFileProperties(
                ReaderUtils.IsFlagSet(tes4.Flag, 0x00000001),
                ReaderUtils.IsFlagSet(tes4.Flag, 0x00000080),
                ReaderUtils.IsFlagSet(tes4.Flag, 0x00000200));
        }
    }
}