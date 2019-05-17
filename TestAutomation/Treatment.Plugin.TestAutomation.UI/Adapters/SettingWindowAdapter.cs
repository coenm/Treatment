namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;

    using JetBrains.Annotations;

    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.WindowControl;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Interfaces;
    using Treatment.Plugin.TestAutomation.UI.Reflection;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Window;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;
    using Treatment.UI.View;

    internal class SettingWindowAdapter : ITestAutomationSettingWindow, ISettingWindow
    {
        [NotNull] private readonly ControlEventPublisher publisher;
        [NotNull] private readonly List<IInitializable> helpers;
        [NotNull] private readonly SettingsWindow settingsWindow;
        [NotNull] private readonly IEventPublisher eventPublisher;
        [NotNull] private ITestAutomationAgent agent;
        [CanBeNull] private ITestAutomationButton browseRootDirectory;
        [CanBeNull] private ITestAutomationTextBox rootDirectory;

        public SettingWindowAdapter([NotNull] SettingsWindow settingsWindow, [NotNull] IEventPublisher eventPublisher, [NotNull] ITestAutomationAgent agent)
        {
            this.settingsWindow = settingsWindow ?? throw new ArgumentNullException(nameof(settingsWindow));
            this.eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            this.agent = agent ?? throw new ArgumentNullException(nameof(agent));

            Guid = Guid.NewGuid();

            publisher = new ControlEventPublisher(this, Guid, eventPublisher);

            helpers = new List<IInitializable>
                      {
                          new InitializedHelper(settingsWindow, c => Initialized?.Invoke(this, c)),
                          new WindowClosingHelper(settingsWindow, c => WindowClosing?.Invoke(this, c)),
                          new WindowClosedHelper(settingsWindow, c => WindowClosed?.Invoke(this, c)),
                          new WindowActivatedDeactivatedHelper(
                                                               settingsWindow,
                                                               callback => WindowActivated?.Invoke(this, callback),
                                                               callback => WindowDeactivated?.Invoke(this, callback)),
                          new PositionChangedHelper(settingsWindow, c => PositionUpdated?.Invoke(this, c)),
                          new SizeChangedHelper(settingsWindow, c => SizeUpdated?.Invoke(this, c)),
                          new EnabledChangedHelper(settingsWindow, c => IsEnabledChanged?.Invoke(this, c)),
                          new FocusHelper(
                                          settingsWindow,
                                          c => FocusableChanged?.Invoke(this, c),
                                          c => GotFocus?.Invoke(this, c),
                                          c => LostFocus?.Invoke(this, c)),
                      };

            eventPublisher.PublishNewControlCreatedAsync(Guid, typeof(ISettingWindow));
        }

        public event EventHandler<Initialized> Initialized;

        public event EventHandler<WindowClosing> WindowClosing;

        public event EventHandler<WindowClosed> WindowClosed;

        public event EventHandler<WindowActivated> WindowActivated;

        public event EventHandler<WindowDeactivated> WindowDeactivated;

        public event EventHandler<PositionUpdated> PositionUpdated;

        public event EventHandler<IsEnabledChanged> IsEnabledChanged;

        public event EventHandler<SizeUpdated> SizeUpdated;

        public event EventHandler<FocusableChanged> FocusableChanged;

        public event EventHandler<GotFocus> GotFocus;

        public event EventHandler<LostFocus> LostFocus;

        public Guid Guid { get; }

        public IButton BrowseRootDirectory => browseRootDirectory;

        public ITextBox RootDirectory => rootDirectory;

        public void Dispose()
        {
            helpers.ForEach(helper => helper.Dispose());
            helpers.Clear();
            publisher.Dispose();
        }

        public void Initialize()
        {
            helpers.ForEach(helper => helper.Initialize());

            UpdateBrowseRootDirectory(
                new ButtonAdapter(
                    FieldsHelper.FindFieldInUiElementByName<Button>(settingsWindow, nameof(BrowseRootDirectory)),
                    eventPublisher));
            browseRootDirectory?.Initialize();

            UpdateRootDirectory(
                new TextBoxAdapter(
                    FieldsHelper.FindFieldInUiElementByName<TextBox>(settingsWindow, nameof(RootDirectory)),
                    eventPublisher));
            rootDirectory?.Initialize();
        }

        private void UpdateBrowseRootDirectory(ITestAutomationButton value)
        {
            browseRootDirectory = value;

            if (value != null)
                eventPublisher.PublishAssignedAsync(Guid, nameof(BrowseRootDirectory), value.Guid);
            else
                eventPublisher.PublishClearedAsync(Guid, nameof(BrowseRootDirectory));
        }

        private void UpdateRootDirectory(ITestAutomationTextBox value)
        {
            rootDirectory = value;

            if (value != null)
                eventPublisher.PublishAssignedAsync(Guid, nameof(RootDirectory), value.Guid);
            else
                eventPublisher.PublishClearedAsync(Guid, nameof(RootDirectory));
        }
    }
}
