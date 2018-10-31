namespace Treatment.UI
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

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
    using Treatment.UI.Model;
    using Treatment.UI.View;
    using Treatment.UI.ViewModel;
    using Treatment.UI.ViewModel.Settings;

    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            var container = Bootstrap();

            RunApplication(container);
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
            container.Register<IMainWindowViewModel, MainWindowViewModel>();
            container.Register<IProjectCollectionViewModel, ProjectCollectionViewModel>(Lifestyle.Scoped);
            container.Register<IEntityEditorViewModel<ApplicationSettings>, ApplicationSettingsViewModel>(Lifestyle.Scoped);
            container.Register<IStatusViewModel, StatusViewModel>(Lifestyle.Scoped);

            container.Register<IStatusModel, StatusModel>(Lifestyle.Scoped);

            container.RegisterSingleton<IEntityEditor, EditEntityInDialog>();

            container.RegisterSingleton<IConfigurationService, FileBasedConfigurationService>();
            container.RegisterDecorator<IConfigurationService, CacheConfigurationServiceDecorator>(Lifestyle.Singleton);
            container.RegisterDecorator<IConfigurationService, ConcurrentConfigurationServiceDecorator>(Lifestyle.Singleton);

            container.Register<IProjectViewModelFactory, ProjectViewModelFactory>(Lifestyle.Scoped);

            container.Register<IUserInterfaceSynchronizationContextProvider, UserInterfaceSynchronizationContextProvider>(Lifestyle.Singleton);

            RegisterPlugins(container);

            RegisterUserInterfaceDependencies(container);

            RegisterDebug(container);

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
                    var app = new App();

                    app.Startup += (sender, args) =>
                    {
                        // stupid workaround to capture the UI thread and to store it in a singleton
                        var contextProvider = container.GetInstance<IUserInterfaceSynchronizationContextProvider>();
                        ((UserInterfaceSynchronizationContextProvider)contextProvider).Set(SynchronizationContext.Current);
                    };

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
