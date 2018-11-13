namespace Treatment.UI
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Threading;

    using JetBrains.Annotations;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using Treatment.Core.Bootstrap;
    using Treatment.Core.Bootstrap.Plugin;
    using Treatment.Core.DefaultPluginImplementation.FileSearch;
    using Treatment.Helpers.Guards;
    using Treatment.UI.Core.Configuration;
    using Treatment.UI.Framework;
    using Treatment.UI.Framework.SynchronizationContext;
    using Treatment.UI.Framework.View;
    using Treatment.UI.Framework.ViewModel;
    using Treatment.UI.Implementations.Configuration;
    using Treatment.UI.Implementations.Delay;
    using Treatment.UI.Model;
    using Treatment.UI.View;
    using Treatment.UI.ViewModel;

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
            // Create the container as usual.
            var container = new Container();

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Options.AllowOverridingRegistrations = true;

            CoreBootstrap.Bootstrap(container);

            // Views
            container.Register<MainWindow>();
            container.Register<IEntityEditorView<ApplicationSettings>, SettingsWindow>();

            // View models
            container.Register<IMainWindowViewModel, MainWindowViewModel>(Lifestyle.Scoped);
            container.Register<IProjectCollectionViewModel, ProjectCollectionViewModel>(Lifestyle.Scoped);
            container.Register<IStatusViewModel, StatusViewModel>(Lifestyle.Scoped);
            container.Register<IEntityEditorViewModel<ApplicationSettings>, ApplicationSettingsViewModel>(Lifestyle.Scoped);

            // not sure if both are the same instance.
            container.Register<IStatusReadModel, StatusModel>(Lifestyle.Singleton);
            container.Register<IStatusFullModel, StatusModel>(Lifestyle.Singleton);
            container.RegisterDecorator<IStatusFullModel, StatusModelLogDecorator>(Lifestyle.Singleton);

            container.RegisterSingleton<IModelEditor, EditModelInDialog>();

            container.RegisterSingleton<IReadOnlyConfigurationService, FileBasedConfigurationService>();
            container.RegisterSingleton<IConfigurationService, FileBasedConfigurationService>();
            container.RegisterDecorator<IConfigurationService, CacheConfigurationServiceDecorator>(Lifestyle.Singleton);
            container.RegisterDecorator<IConfigurationService, ConcurrentConfigurationServiceDecorator>(Lifestyle.Singleton);

            container.Register<IConfigFilenameProvider, AppConfigFilenameProvider>(Lifestyle.Singleton);
            container.RegisterDecorator<IConfigFilenameProvider, VerifyAndFixFilenameDecorator>(Lifestyle.Singleton);
            container.RegisterDecorator<IConfigFilenameProvider, UpdateStatusModelDecorator>(Lifestyle.Singleton);

            container.Register<IProjectViewModelFactory, ProjectViewModelFactory>(Lifestyle.Scoped);

            container.Register<IUserInterfaceSynchronizationContextProvider, UserInterfaceSynchronizationContextProvider>(Lifestyle.Singleton);

            RegisterPlugins(container);

            RegisterUserInterfaceDependencies(container);

            RegisterDelay(container);

            container.RegisterSingleton<DispatcherObject, App>();
            container.RegisterSingleton<Application, App>();

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
            catch
            {
                // Log the exception and exit
#if DEBUG
                throw;
#endif
            }
        }
    }
}
