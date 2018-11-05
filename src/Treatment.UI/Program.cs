namespace Treatment.UI
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Threading;
    using Implementations.Configuration;
    using JetBrains.Annotations;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using Treatment.Core.Bootstrap;
    using Treatment.Core.DefaultPluginImplementation.FileSearch;
    using Treatment.Helpers;
    using Treatment.UI.Core;
    using Treatment.UI.Core.Configuration;
    using Treatment.UI.Framework;
    using Treatment.UI.Framework.SynchronizationContext;
    using Treatment.UI.Framework.View;
    using Treatment.UI.Framework.ViewModel;
    using Treatment.UI.Model;
    using Treatment.UI.View;
    using Treatment.UI.ViewModel;
    using Treatment.UI.ViewModel.Settings;

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

            // Register your windows and view models:
            container.Register<MainWindow>();
            container.Register<IEntityEditorView<ApplicationSettings>, SettingsWindow>();

            // View Models.
            container.Register<IMainWindowViewModel, MainWindowViewModel>(Lifestyle.Scoped);
            container.Register<IProjectCollectionViewModel, ProjectCollectionViewModel>(Lifestyle.Scoped);
            container.Register<IStatusViewModel, StatusViewModel>(Lifestyle.Scoped);
            container.Register<IEntityEditorViewModel<ApplicationSettings>, ApplicationSettingsViewModel>(Lifestyle.Scoped);

            // not sure..
            container.Register<IStatusReadModel, StatusModel>(Lifestyle.Scoped);
            container.Register<IStatusFullModel, StatusModel>(Lifestyle.Scoped);

            container.RegisterSingleton<IModelEditor, EditModelInDialog>();

            container.RegisterSingleton<IConfigurationService, FileBasedConfigurationService>();
            container.RegisterDecorator<IConfigurationService, CacheConfigurationServiceDecorator>(Lifestyle.Singleton);
            container.RegisterDecorator<IConfigurationService, ConcurrentConfigurationServiceDecorator>(Lifestyle.Singleton);

            container.RegisterSingleton<IConfigFilenameProvider, AppConfigFilenameProvider>();

            container.Register<IProjectViewModelFactory, ProjectViewModelFactory>(Lifestyle.Scoped);

            container.Register<IUserInterfaceSynchronizationContextProvider, UserInterfaceSynchronizationContextProvider>(Lifestyle.Singleton);

            RegisterPlugins(container);

            RegisterUserInterfaceDependencies(container);

            RegisterDebug(container);

            container.RegisterSingleton<DispatcherObject, App>();
            container.RegisterSingleton<Application, App>();

            container.Verify(VerificationOption.VerifyAndDiagnose);

            return container;
        }

        private static void RegisterUserInterfaceDependencies([NotNull] Container container)
        {
            DebugGuard.NotNull(container, nameof(container));

            container.RegisterSingleton<ISearchProviderNameOption, AppConfigConfiguration>();
            container.RegisterSingleton<IConfiguration, AppConfigConfiguration>();
        }

        private static void RegisterDebug([NotNull] Container container)
        {
            DebugGuard.NotNull(container, nameof(container));

            DelayCommandExecution.Register(container);
        }

        private static void RegisterPlugins([NotNull] Container container)
        {
            DebugGuard.NotNull(container, nameof(container));

            var pluginDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);

            var pluginAssemblies = new DirectoryInfo(pluginDirectory)
                                   .GetFiles()
                                   .Where(file =>
                                              file.Name.StartsWith("Treatment.Plugin.")
                                              &&
                                              file.Extension.ToLower() == ".dll")
                                   .Select(file => Assembly.Load(AssemblyName.GetAssemblyName(file.FullName)));

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
