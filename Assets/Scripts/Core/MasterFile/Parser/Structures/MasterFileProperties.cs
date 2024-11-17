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

        public MasterFileProperties(bool isMaster, bool isLocalized, bool isLight)
        {
            IsMaster = isMaster;
            IsLocalized = isLocalized;
            IsLight = isLight;
        }
    }
}