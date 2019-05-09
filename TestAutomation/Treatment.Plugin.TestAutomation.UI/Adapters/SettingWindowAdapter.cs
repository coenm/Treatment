namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;

    using JetBrains.Annotations;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;
    using Treatment.UI.View;

    internal class SettingWindowAdapter : ISettingWindow, ITestAutomationView
    {
        [NotNull] private SettingsWindow settingsWindow;
        [NotNull] private IEventPublisher publisher;
        [NotNull] private ITestAutomationAgent agent;

        public SettingWindowAdapter([NotNull] SettingsWindow settingsWindow, [NotNull] IEventPublisher publisher, [NotNull] ITestAutomationAgent agent)
        {
            this.settingsWindow = settingsWindow ?? throw new ArgumentNullException(nameof(settingsWindow));
            this.publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            this.agent = agent ?? throw new ArgumentNullException(nameof(agent));

            Guid = Guid.NewGuid();

            publisher.PublishNewControlCreatedAsync(Guid, typeof(ISettingWindow));
        }

        public Guid Guid { get; }

        public IButton OpenSettingsButton { get; }

        public IProjectListView ProjectList { get; }

        public IMainViewStatusBar StatusBar { get; }

        public void Dispose()
        {
        }

        public void Initialize()
        {
        }
    }
}
