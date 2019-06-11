namespace Treatment.UIStart
{
    using System;
    using System.IO;
    using System.Windows;
    using JetBrains.Annotations;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using Treatment.Core.Bootstrap.Plugin;
    using Treatment.Core.DefaultPluginImplementation.FileSearch;
    using Treatment.Helpers.Guards;
    using Treatment.UI;
    using Treatment.UI.Core.Configuration;
    using Treatment.UI.Implementations.Delay;
    using Treatment.UI.View;

    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using (var container = Bootstrap())
            {
                RunApplication(container);
            }
        }

        private static Container Bootstrap()
        {
            var container = new Container();

            UiBootstrapper.Bootstrap(container);

            if (Environment.GetEnvironmentVariable("ENABLE_TEST_AUTOMATION") == null)
                container.Verify(VerificationOption.VerifyAndDiagnose);

            return container;
        }

        private static void RegisterUserInterfaceDependencies([NotNull] Container container)
        {
            DebugGuard.NotNull(container, nameof(container));
            container.RegisterSingleton<ISearchProviderNameOption, AppConfigConfiguration>();
        }

        private static void RegisterDelay([NotNull] Container container)
        {
            DebugGuard.NotNull(container, nameof(container));
            DelayCommandExecution.Register(container);
        }

        private static void RegisterPlugins([NotNull] Container container)
        {
            DebugGuard.NotNull(container, nameof(container));

            var pluginAssemblies = PluginFinder.FindPluginAssemblies(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
            container.RegisterPackages(pluginAssemblies);
        }

        private static void RunApplication([NotNull] Container container)
        {
            DebugGuard.NotNull(container, nameof(container));

            try
            {
                using (AsyncScopedLifestyle.BeginScope(container))
                {
                    var app = container.GetInstance<Application>();
                    var mainWindow = container.GetInstance<MainWindow>();

                    void MainWindowOnClosed(object sender, EventArgs e)
                    {
                        mainWindow.Closed -= MainWindowOnClosed;
                        app.Shutdown(0);
                    }

                    // not sure if this is the way to do this.
                    mainWindow.Closed += MainWindowOnClosed;

                    app.Run(mainWindow);
                }
            }

            // ReSharper disable once RedundantCatchClause
            catch (Exception ex)
            {
                // Log the exception and exit
                Console.WriteLine(ex.Message);
#if DEBUG
                // throw;
#endif
            }
        }
    }
}
