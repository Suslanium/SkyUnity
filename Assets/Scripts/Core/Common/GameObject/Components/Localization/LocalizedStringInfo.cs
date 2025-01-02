namespace Core.Common.GameObject.Components.Localization
{
    public class LocalizedStringInfo
    {
        public readonly string Fallback;
        public readonly uint Index;
        public readonly string PluginName;
        public readonly string NonLocalizedVariant;
        public readonly bool IsNotLocalized;

        public LocalizedStringInfo(string fallback, uint index, string pluginName, string nonLocalizedVariant,
            bool isNotLocalized)
        {
            Fallback = fallback;
            Index = index;
            PluginName = pluginName;
            NonLocalizedVariant = nonLocalizedVariant;
            IsNotLocalized = isNotLocalized;
        }
    }
}