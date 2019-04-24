namespace Treatment.Plugin.TestAutomation.UI
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;
    using SimpleInjector.Advanced;
    using SimpleInjector.Packaging;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Settings;
    using Treatment.UI.View;
    using ZeroMQ;

    using Container = SimpleInjector.Container;

    [UsedImplicitly]
    public class TestAutomationPackage : IPackage
    {
        private Container container;

        public void RegisterServices([CanBeNull] Container container)
        {
            if (container == null)
                return;

            ITestAutomationSettings settings = new EnvironmentVariableSettings();

            if (settings.TestAutomationEnabled == false && Environment.UserInteractive)
                settings = new DummySettings();

            if (settings.TestAutomationEnabled == false)
                return;

            this.container = container;

            container.RegisterInstance<ITestAutomationSettings>(settings);
            container.RegisterSingleton<IEventPublisher, ZeroMqEventPublisher>();
            container.RegisterSingleton<IZeroMqContextService, ZeroMqContextService>();

            container.RegisterSingleton<ITestAutomationAgent, TestAutomationAgent>();

            container.Options.RegisterResolveInterceptor(CollectResolvedMainWindowInstance, c => c.Producer.ServiceType == typeof(MainWindow));
        }

        private object CollectResolvedMainWindowInstance(InitializationContext context, Func<object> instanceProducer)
        {
            var instance = instanceProducer();

            if (!(instance is MainWindow mainWindow))
                return instance;

            var publisher = container.GetInstance<IEventPublisher>();
            var agent = container.GetInstance<ITestAutomationAgent>();

            for (var i = 3; i >= 0; i--)
            {
                publisher.PublishAsync(new TestAutomationEvent
                {
                    Control = $"Start in {i} seconds",
                });

                if (i > 0)
                    Thread.Sleep(1000);
            }

            agent.RegisterMainView(new MainWindowTestAutomationView(mainWindow, publisher, agent));
            // agent.StartAccepting();

            return instance;
        }
    }

    internal interface ITestAutomationAgent
    {
        void RegisterMainView([NotNull] MainWindowTestAutomationView instance);

        void StartAccepting();

        void Stop();
    }

    internal class TestAutomationAgent : ITestAutomationAgent
    {
        private MainWindowTestAutomationView instance;
        [CanBeNull] private Task task;

        [NotNull] private readonly ITestAutomationSettings settings;
        [NotNull] private readonly object syncLock = new object();
        [NotNull] private readonly ZContext context;
        [CanBeNull] private ZSocket socket;

        private CancellationTokenSource cts;

        public TestAutomationAgent([NotNull] IZeroMqContextService contextService, [NotNull] ITestAutomationSettings settings)
        {
            Guard.NotNull(contextService, nameof(contextService));
            Guard.NotNull(settings, nameof(settings));

            this.settings = settings;
            context = contextService.GetContext() ?? throw new NullReferenceException();
        }

        public void StartAccepting()
        {
            cts = new CancellationTokenSource();
            task = Task.Run(() =>
            {
                Initialize();
                while (cts.IsCancellationRequested == false)
                {
                    ZMessage msg = null;
                    ZError error = null;

                    if (socket.ReceiveMessage(ref msg, ZSocketFlags.DontWait, out error))
                    {
                        var stringmessage = msg[0].ReadString();

                        try
                        {
//                            .WithReferences(typeof(MainWindowTestAutomationView).Assembly)
//                                .WithImports("Treatment.Plugin.TestAutomation.UI.Adapters")

                            var result = CSharpScript.EvaluateAsync(
                                "2+2",
                                ScriptOptions.Default,
                                instance,
                                typeof(MainWindowTestAutomationView)).GetAwaiter().GetResult();
                            socket.SendMessage(new ZMessage
                            {
                                new ZFrame(stringmessage),
                                new ZFrame(result.ToString()),
                            });
                        }
                        catch (CompilationErrorException e)
                        {
                            socket.SendMessage(new ZMessage
                            {
                                new ZFrame(stringmessage),
                                new ZFrame(e.Diagnostics.ToString()),
                            });
                        }
                        catch (Exception e)
                        {
                            socket.SendMessage(new ZMessage
                            {
                                new ZFrame(stringmessage),
                                new ZFrame("AI: " + e.Message.ToString()),
                            });
                        }

//                        var value = CSharpScript.Eval("X + Y", new Globals { X = 1, Y = 2 });
                    }
                }

                socket?.Dispose();
                socket = null;

                cts = null;
            });
        }

        public void Stop()
        {
            cts?.Cancel();
            instance = null;
        }

        public void RegisterMainView([NotNull] MainWindowTestAutomationView instance)
        {
            Guard.NotNull(instance, nameof(instance));
            this.instance = instance;
        }

        private void Initialize()
        {
            if (socket != null)
                return;

            lock (syncLock)
            {
                if (socket != null)
                    return;

                socket = new ZSocket(context, ZSocketType.REP)
                {
                    Linger = TimeSpan.Zero,
                };

                socket.Bind(settings.ZeroMqRequestResponseSocket);

                Thread.Sleep(1);
            }
        }
    }
}
