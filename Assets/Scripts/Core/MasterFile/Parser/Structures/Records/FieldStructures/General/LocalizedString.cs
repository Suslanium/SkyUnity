using Core.Common.GameObject.Components.Localization;

namespace Core.MasterFile.Parser.Structures.Records.FieldStructures.General
{
    public class LocalizedString
    {
        public readonly bool IsLoaded;
        public readonly uint Index;
        public readonly string PluginName;
        public readonly string Value;

        public LocalizedString(uint index, string pluginName)
        {
            Index = index;
            PluginName = pluginName;
            Value = null;
            IsLoaded = false;
        }

        public LocalizedString(string value, string pluginName)
        {
            Index = 0;
            PluginName = pluginName;
            Value = value;
            IsLoaded = true;
        }

        public LocalizedStringInfo ToLocalizedStringInfo(string fallback)
        {
            return new LocalizedStringInfo(
                fallback,
                Index,
                PluginName,
                Value,
                IsLoaded);
        }
    }
}