namespace TestAgent
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    using JetBrains.Annotations;
    using NLog;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using TestAgent.View;
    using TestAgent.ViewModel;
    using Treatment.Helpers.Guards;
    using Wpf.Framework.SynchronizationContext;

    public static class Program
    {
#if DEBUG
        private const string Config = "Debug";
#else
        private const string Config = "Release";
#endif

        private static Container container;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [STAThread]
        public static void Main(string[] args)
        {
            using (container = new Container())
            {
                container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
                container.Options.AllowOverridingRegistrations = true;

                Bootstrapper.Bootstrap(
                    container,
                    $"tcp://*:{FixedSettings.AgentReqRspPort}",
                    $"tcp://*:{FixedSettings.AgentPublishPort}",
                    FixedSettings.SutPublishPort,
                    FixedSettings.SutReqRspPort);

                // Views
                container.Register<MainWindow>();

                // View models
                container.Register<ITestAgentMainWindowViewModel, TestAgentMainWindowViewModel>(Lifestyle.Transient);

                container
                    .Register<IUserInterfaceSynchronizationContextProvider, UserInterfaceSynchronizationContextProvider
                    >(Lifestyle.Singleton);
                container.RegisterSingleton<DispatcherObject, App>();
                container.RegisterSingleton<Application, App>();

                container.Verify(VerificationOption.VerifyOnly);

                RunApplication(container);
            }
        }

        private static void RunApplication([NotNull] Container container)
        {
            DebugGuard.NotNull(container, nameof(container));

            try
            {
//                using (AsyncScopedLifestyle.BeginScope(container))
                {
                    ManualResetEvent mre = new ManualResetEvent(false);
                    var app = container.GetInstance<Application>();
                    var mainWindow = container.GetInstance<MainWindow>();

                    void MainWindowOnClosed(object sender, EventArgs e)
                    {
                        mainWindow.Closed -= MainWindowOnClosed;
                        app.Shutdown(0);
                    }

                    // not sure if this is the way to do this.
                    mainWindow.Closed += MainWindowOnClosed;

                    var result = app.Run(mainWindow);
                }
            }

            // ReSharper disable once RedundantCatchClause
            catch (Exception ex)
            {
                // Log the exception and exit
                Logger.Fatal(() => "Exception occurred.");
                Logger.Fatal(() => ex.Message);
                Logger.Fatal(() => ex.StackTrace);
#if DEBUG
                throw;
#endif

            }
        }

    }
}
