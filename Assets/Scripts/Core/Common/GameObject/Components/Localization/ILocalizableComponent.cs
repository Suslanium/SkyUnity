namespace Core.Common.GameObject.Components.Localization
{
    public interface ILocalizableComponent : IUnprocessedComponent
    {
        public LocalizedStringInfo LocalizedStringInfo { get; }
        
        public IComponent GetLocalizedComponent(string resolvedLocalizedString);
    }
}