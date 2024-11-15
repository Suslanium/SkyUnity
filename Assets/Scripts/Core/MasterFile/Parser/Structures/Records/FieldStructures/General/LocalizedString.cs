namespace Core.MasterFile.Parser.Structures.Records.FieldStructures.General
{
    public class LocalizedString
    {
        public readonly bool IsLoaded;
        public readonly uint Index;
        public readonly string Value;

        public LocalizedString(uint index)
        {
            Index = index;
            Value = null;
            IsLoaded = false;
        }
        
        public LocalizedString(string value)
        {
            Index = 0;
            Value = value;
            IsLoaded = true;
        }
    }
}