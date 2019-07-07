namespace TestAgent.ViewModel
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Windows.Input;

    using CoenM.ZeroMq.Socket;
    using CoenM.ZeroMq.Worker;
    using JetBrains.Annotations;
    using Nito.Mvvm;
    using NLog;
    using TestAgent.Contract.Interface.Events;
    using TestAgent.Model.Configuration;
    using TestAgent.ZeroMq;
    using TestAgent.ZeroMq.PublishInfrastructure;
    using TestAgent.ZeroMq.RequestReplyInfrastructure;
    using Treatment.Helpers.Guards;
    using Wpf.Framework.EntityEditor;
    using Wpf.Framework.SynchronizationContext;
    using Wpf.Framework.ViewModel;

    public class TestAgentMainWindowViewModel : ViewModelBase, ITestAgentMainWindowViewModel, IDisposable
    {
        [NotNull] private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        [NotNull] private readonly IAgentContext agent;
        [NotNull] private readonly CancellationTokenSource cts;
        [NotNull] private readonly ZeroMqReqRepProxyService reqRspProxy;
        [NotNull] private readonly ZeroMqPublishProxyService publishProxy;
        [NotNull] private readonly CompositeDisposable disposable;
        [NotNull] private readonly ITestAgentEventPublisher eventPublisher;
        [NotNull] private readonly EventsRx eventsProcessor;

        /* Shitload of dependencies... fix this.. */
        public TestAgentMainWindowViewModel(
            [NotNull] IZeroMqReqRepProxyFactory zeroMqReqRepProxyFactory,
            [NotNull] IZeroMqPublishProxyFactory zeroMqPublishProxyFactory,
            [NotNull] ReqRepWorkerManagement workerManager,
            [NotNull] IZeroMqRequestDispatcher zmqDispatcher,
            [NotNull] IZeroMqSocketFactory socketFactory,
            [NotNull] IUserInterfaceSynchronizationContextProvider uiContextProvider,
            [NotNull] IAgentContext agent,
            [NotNull] IModelEditor modelEditor,
            [NotNull] IConfigurationService configurationService,
            [NotNull] ITestAgentEventPublisher eventPublisher)
        {
            Guard.NotNull(zeroMqReqRepProxyFactory, nameof(zeroMqReqRepProxyFactory));
            Guard.NotNull(zeroMqPublishProxyFactory, nameof(zeroMqPublishProxyFactory));
            Guard.NotNull(workerManager, nameof(workerManager));
            Guard.NotNull(zmqDispatcher, nameof(zmqDispatcher));
            Guard.NotNull(socketFactory, nameof(socketFactory));
            Guard.NotNull(uiContextProvider, nameof(uiContextProvider));
            Guard.NotNull(agent, nameof(agent));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.agent = agent;
            this.eventPublisher = eventPublisher;

            cts = new CancellationTokenSource();

            reqRspProxy = zeroMqReqRepProxyFactory.Create();
            reqRspProxy.Start();

            publishProxy = zeroMqPublishProxyFactory.Create();
            publishProxy.Start();

            var workerTask1 = workerManager.StartSingleWorker(
                zmqDispatcher,
                FixedSettings.InternalRequestResponseWorkerSocket,
                cts.Token);

            eventsProcessor = new EventsRx(socketFactory, FixedSettings.InternalPublishProxyCapturingSocket);

            disposable = new CompositeDisposable
            {
                eventsProcessor.Events
                    .Buffer(TimeSpan.FromMilliseconds(100))
                    .ObserveOn(uiContextProvider.UiSynchronizationContext)
                    .Subscribe(ev => { EventsCounter += ev.Count; }),
            };

            OpenSettingsCommand = new CapturingExceptionAsyncCommand(async () =>
            {
                try
                {
                    Logger.Debug("Try get application settings.");
                    var applicationSettings = await configurationService.GetAsync();

                    Logger.Debug("get modeleditor for application settings.");
                    var result = modelEditor.Edit(applicationSettings);
                    if (result.HasValue && result.Value)
                    {
                        Logger.Debug("Try to update the settings.");
                        await configurationService.UpdateAsync(applicationSettings);
                    }

                    Logger.Debug("Done");
                }
                catch (Exception e)
                {
                    Logger.Error(e, e.Message);
                }
            });

            // let listeners know agent has started.
            eventPublisher.PublishAsync(new TestAgentStarted());
        }

        public int EventsCounter
        {
            get => Properties.Get(0);
            private set => Properties.Set(value);
        }

        public ICommand OpenSettingsCommand { get; }

        public void Dispose()
        {
            eventPublisher.PublishAsync(new TestAgentStopped());
            disposable.Dispose();
            eventsProcessor.Dispose();
            cts.Cancel();
            agent.Stop();
            publishProxy.Dispose();
            reqRspProxy.Dispose();
        }
    }
}
