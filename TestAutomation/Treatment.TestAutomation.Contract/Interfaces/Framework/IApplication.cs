namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using JetBrains.Annotations;
    using Treatment;

    public interface IApplication : IUiElement
    {
        IMainView MainView { get; }

        void RegisterAndInitializeMainView([NotNull] IMainView mainView);
    }
}
