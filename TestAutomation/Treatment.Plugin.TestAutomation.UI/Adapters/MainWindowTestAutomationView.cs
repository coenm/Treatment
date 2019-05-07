﻿namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.WindowControl;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Reflection;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;
    using Treatment.UI.UserControls;
    using Treatment.UI.View;

    internal class MainWindowTestAutomationView : IMainView
    {
        [NotNull] private readonly MainWindow mainWindow;
        [NotNull] private readonly IEventPublisher eventPublisher;
        [NotNull] private readonly List<IInitializable> helpers;

        public MainWindowTestAutomationView(
            [NotNull] MainWindow mainWindow,
            [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(mainWindow, nameof(mainWindow));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            Guid = Guid.NewGuid();

            this.mainWindow = mainWindow;
            this.eventPublisher = eventPublisher;

            helpers = new List<IInitializable>
                      {
                          new WindowInitializedHelper(mainWindow, eventPublisher, Guid),
                          new WindowClosingHelper(mainWindow, eventPublisher, Guid),
                          new WindowClosedHelper(mainWindow, eventPublisher, Guid),
                          new WindowActivatedDeactivatedHelper(mainWindow, eventPublisher, Guid),
                      };
        }

        public IButton OpenSettingsButton { get; private set; }

        public IProjectListView ProjectList { get; private set; }

        public IMainViewStatusBar StatusBar { get; private set; }

        public Guid Guid { get; }

        public void Dispose()
        {
            helpers.ForEach(helper => helper.Dispose());
        }

        public void Initialize()
        {
            helpers.ForEach(helper => helper.Initialize());

            OpenSettingsButton = new ButtonAdapter(
                FieldsHelper.FindFieldInUiElementByName<Button>(mainWindow, nameof(OpenSettingsButton)),
                eventPublisher);
            eventPublisher.PublishNewControl(OpenSettingsButton.Guid, typeof(ButtonAdapter), Guid);
            OpenSettingsButton.Initialize();

            StatusBar = new MainViewStatusBarAdapter(
                FieldsHelper.FindFieldInUiElementByName<StatusBar>(mainWindow, nameof(StatusBar)),
                eventPublisher);
            eventPublisher.PublishNewControl(StatusBar.Guid, typeof(MainViewStatusBarAdapter), Guid);
            StatusBar.Initialize();

            ProjectList = new ProjectListViewAdapter(
                FieldsHelper.FindFieldInUiElementByName<ProjectListView>(mainWindow, nameof(ProjectList)),
                eventPublisher);
            eventPublisher.PublishNewControl(ProjectList.Guid, typeof(ProjectListViewAdapter), Guid);
            ProjectList.Initialize();
        }

        public event CancelEventHandler Closing
        {
            add => mainWindow.Closing += value;
            remove => mainWindow.Closing -= value;
        }
    }
}
