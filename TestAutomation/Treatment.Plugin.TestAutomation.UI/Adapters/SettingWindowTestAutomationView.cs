namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;

    using JetBrains.Annotations;

    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.UI.View;

    internal class SettingWindowTestAutomationView : ITestAutomationView
    {
        [NotNull] private SettingsWindow settingsWindow;
        [NotNull] private IEventPublisher publisher;
        [NotNull] private ITestAutomationAgent agent;

        public SettingWindowTestAutomationView([NotNull] SettingsWindow settingsWindow, [NotNull] IEventPublisher publisher, [NotNull] ITestAutomationAgent agent)
        {
            this.settingsWindow = settingsWindow ?? throw new ArgumentNullException(nameof(settingsWindow));
            this.publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            this.agent = agent ?? throw new ArgumentNullException(nameof(agent));

            Guid = Guid.NewGuid();
        }

        public Guid Guid { get; }

        public void Dispose()
        {
        }

        public void Initialize()
        {
        }
    }
}
