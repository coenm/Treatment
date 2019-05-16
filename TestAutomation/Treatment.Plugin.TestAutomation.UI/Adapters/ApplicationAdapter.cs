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
        [NotNull] private readonly ControlEventPublisher publisher;

        public ApplicationAdapter(
            [NotNull] Application item,
            [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;
            this.eventPublisher = eventPublisher;

            var guid = Guid.NewGuid();

            publisher = new ControlEventPublisher(this, guid, eventPublisher);

            eventPublisher.PublishAsync(new ApplicationStarting
            {
                CountDown = 0,
            });

            helpers = new List<IInitializable>
                      {
                          new ApplicationActivationHelper(
                                                          item,
                                                          c => Activated?.Invoke(this, c),
                                                          c => Deactivated?.Invoke(this, c)),
                          new ApplicationStartupHelper(item, c => Startup?.Invoke(this, c)),
                          new ApplicationExitHelper(item, c => Exit?.Invoke(this, c)),
                          new ApplicationDispatcherUnhandledExceptionHelper(item, eventPublisher, guid),
                      };

            eventPublisher.PublishNewControlCreatedAsync(guid, typeof(IApplication));
        }

        public event EventHandler<ApplicationActivated> Activated;

        public event EventHandler<ApplicationDeactivated> Deactivated;

        public event EventHandler<ApplicationExit> Exit;

        public event EventHandler<ApplicationStarted> Startup;

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
            publisher.Dispose();
        }
    }
}
