namespace Treatment.Plugin.TestAutomation.UI.Adapters
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
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Reflection;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Window;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;
    using Treatment.UI.UserControls;
    using Treatment.UI.View;

    internal class MainWindowAdapter : IMainView
    {
        [NotNull] private readonly MainWindow mainWindow;
        [NotNull] private readonly IEventPublisher eventPublisher;
        [NotNull] private readonly List<IInitializable> helpers;
        [NotNull] private readonly ControlEventPublisher publisher;

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

            eventPublisher.PublishNewControlCreatedAsync(Guid, typeof(IMainView));
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

        public IButton OpenSettingsButton { get; private set; }

        public IProjectListView ProjectList { get; private set; }

        public IMainViewStatusBar StatusBar { get; private set; }

        public Guid Guid { get; }

        public void Dispose()
        {
            helpers.ForEach(helper => helper.Dispose());

            publisher.Dispose();
        }

        public void Initialize()
        {
            helpers.ForEach(helper => helper.Initialize());

            OpenSettingsButton = new ButtonAdapter(
                FieldsHelper.FindFieldInUiElementByName<Button>(mainWindow, nameof(OpenSettingsButton)),
                eventPublisher);
            ((ButtonAdapter)OpenSettingsButton).Initialize();

            StatusBar = new MainViewStatusBarAdapter(
                FieldsHelper.FindFieldInUiElementByName<StatusBar>(mainWindow, nameof(StatusBar)),
                eventPublisher);
            StatusBar.Initialize();

            ProjectList = new ProjectListViewAdapter(
                FieldsHelper.FindFieldInUiElementByName<ProjectListView>(mainWindow, nameof(ProjectList)),
                eventPublisher);
            ProjectList.Initialize();
        }
    }
}
