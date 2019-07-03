namespace Treatment.Plugin.TestAutomation.UI.Adapters.TreatmentControls
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.WindowControl;
    using Treatment.Plugin.TestAutomation.UI.Adapters.WindowsControls;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Interfaces;
    using Treatment.Plugin.TestAutomation.UI.Reflection;
    using Treatment.TestAutomation.Contract.Interfaces.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Window;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.UI.Core.UserControls;
    using Treatment.UI.Core.View;

    internal class MainWindowAdapter : ITestAutomationMainWindow, IMainWindow
    {
        [NotNull] private readonly MainWindow mainWindow;
        [NotNull] private readonly IEventPublisher eventPublisher;
        [NotNull] private readonly List<IInitializable> helpers;
        [NotNull] private readonly ControlEventPublisher publisher;
        [CanBeNull] private ITestAutomationMainViewStatusBar statusBar;
        [CanBeNull] private ITestAutomationButton openSettingsButton;
        [CanBeNull] private ITestAutomationProjectListView projectList;

        public MainWindowAdapter(
            [NotNull] MainWindow mainWindow,
            [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(mainWindow, nameof(mainWindow));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            Guid = Guid.NewGuid();

            this.mainWindow = mainWindow;
            this.eventPublisher = eventPublisher;

            publisher = new ControlEventPublisher(this, Guid, eventPublisher);

            helpers = new List<IInitializable>
                      {
                          new InitializedHelper(mainWindow, c => Initialized?.Invoke(this, c)),
                          new WindowClosingHelper(mainWindow, c => WindowClosing?.Invoke(this, c)),
                          new WindowClosedHelper(mainWindow, c => WindowClosed?.Invoke(this, c)),
                          new WindowActivatedDeactivatedHelper(
                                                               mainWindow,
                                                               callback => WindowActivated?.Invoke(this, callback),
                                                               callback => WindowDeactivated?.Invoke(this, callback)),
                          new PositionChangedHelper(mainWindow, c => PositionUpdated?.Invoke(this, c)),
                          new SizeChangedHelper(mainWindow, c => SizeUpdated?.Invoke(this, c)),
                          new EnabledChangedHelper(mainWindow, c => IsEnabledChanged?.Invoke(this, c)),
                          new FocusHelper(
                                          mainWindow,
                                          c => FocusableChanged?.Invoke(this, c),
                                          c => GotFocus?.Invoke(this, c),
                                          c => LostFocus?.Invoke(this, c)),
                      };

            eventPublisher.PublishNewControlCreatedAsync(Guid, typeof(IMainWindow));
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

        public IButton OpenSettingsButton => openSettingsButton;

        public IProjectListView ProjectList { get; }

        public IMainViewStatusBar StatusBar => statusBar;

        public Guid Guid { get; }

        public void Dispose()
        {
            helpers.ForEach(helper => helper.Dispose());

            publisher.Dispose();
        }

        public void Initialize()
        {
            helpers.ForEach(helper => helper.Initialize());

            UpdateOpenSettingsButton(
                new ButtonAdapter(
                    FieldsHelper.FindFieldInUiElementByName<Button>(mainWindow, nameof(OpenSettingsButton)),
                    eventPublisher));
            openSettingsButton?.Initialize();

            UpdateStatusBar(
                new MainViewStatusBarAdapter(
                    FieldsHelper.FindFieldInUiElementByName<StatusBar>(mainWindow, nameof(StatusBar)),
                    eventPublisher));
            statusBar?.Initialize();

            UpdateProjectList(
                new ProjectListViewAdapter(
                    FieldsHelper.FindFieldInUiElementByName<ProjectListView>(mainWindow, nameof(ProjectList)),
                    eventPublisher));
            projectList?.Initialize();
        }

        private void UpdateOpenSettingsButton(ITestAutomationButton value)
        {
            openSettingsButton = value;

            if (value != null)
                eventPublisher.PublishAssignedAsync(Guid, nameof(OpenSettingsButton), value.Guid);
            else
                eventPublisher.PublishClearedAsync(Guid, nameof(OpenSettingsButton));
        }

        private void UpdateProjectList(ITestAutomationProjectListView value)
        {
            projectList = value;

            if (value != null)
                eventPublisher.PublishAssignedAsync(Guid, nameof(ProjectList), value.Guid);
            else
                eventPublisher.PublishClearedAsync(Guid, nameof(ProjectList));
        }

        private void UpdateStatusBar(ITestAutomationMainViewStatusBar value)
        {
            statusBar = value;

            if (value != null)
                eventPublisher.PublishAssignedAsync(Guid, nameof(StatusBar), value.Guid);
            else
                eventPublisher.PublishClearedAsync(Guid, nameof(StatusBar));
        }
    }
}
