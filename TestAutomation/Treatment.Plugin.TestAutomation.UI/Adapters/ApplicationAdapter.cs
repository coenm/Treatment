namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.Application;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Interfaces;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;

    public class ApplicationAdapter : ITestAutomationApplication
    {
        [NotNull] private readonly Application item;
        [NotNull] private readonly IEventPublisher eventPublisher;
        [NotNull] private readonly List<IInitializable> helpers;

        public ApplicationAdapter(
            [NotNull] Application item,
            [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;
            this.eventPublisher = eventPublisher;

            Guid = Guid.NewGuid();

            eventPublisher.PublishAsync(new ApplicationStarting
            {
                CountDown = 0,
            });

            helpers = new List<IInitializable>
                      {
                          new ApplicationActivationHelper(item, eventPublisher, Guid),
                          new ApplicationStartupHelper(item, eventPublisher, Guid),
                          new ApplicationExitHelper(item, eventPublisher, Guid),
                          new ApplicationDispatcherUnhandledExceptionHelper(item, eventPublisher, Guid),
                      };

            eventPublisher.PublishNewControlCreatedAsync(Guid, typeof(IApplication));
        }

        /// <summary>Occurs when an application becomes the foreground application.</summary>
        public event EventHandler Activated;

        /// <summary>Occurs when an application stops being the foreground application.</summary>
        public event EventHandler Deactivated;

        /// <summary>Occurs just before an application shuts down, and cannot be canceled.</summary>
        public event EventHandler<ApplicationExit> Exit;

        /// <summary>Occurs when the <see cref="M:System.Windows.Application.Run" /> method of the <see cref="T:System.Windows.Application" /> object is called.</summary>
        public event EventHandler Startup;

        public Guid Guid { get; }

        public IMainView MainView { get; private set; }

        public void Initialize()
        {
            helpers.ForEach(helper => helper.Initialize());
        }

        public void RegisterAndInitializeMainView(IMainView mainView)
        {
            Guard.NotNull(mainView, nameof(mainView));
            MainView = mainView;
            mainView.Initialize();
        }

        public void Dispose()
        {
            helpers.ForEach(helper => helper.Dispose());
        }
    }
}
