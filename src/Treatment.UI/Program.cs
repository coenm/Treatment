namespace Treatment.UI
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using Treatment.Core.Bootstrap;
    using Treatment.Core.DefaultPluginImplementation.FileSearch;
    using Treatment.Helpers;
    using Treatment.UI.Core;
    using Treatment.UI.Core.Configuration;
    using Treatment.UI.Framework;
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

            container.Register<IMainWindowViewModel, MainWindowViewModel>();
            container.Register<IEntityEditorViewModel<ApplicationSettings>, ApplicationSettingsViewModel>();

            container.RegisterSingleton<IShowEntityInDialogProcessor, ShowEntityInDialogProcessor>();

            container.RegisterSingleton<IConfigurationService, FileBasedConfigurationService>();
            container.RegisterDecorator<IConfigurationService, CacheConfigurationServiceDecorator>(Lifestyle.Singleton);
            container.RegisterDecorator<IConfigurationService, ConcurrentConfigurationServiceDecorator>(Lifestyle.Singleton);

            RegisterPlugins(container);

            RegisterUserInterfaceDependencies(container);

            RegisterDebug(container);

            container.Verify();

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
                    var mainWindow = container.GetInstance<MainWindow>();
                    app.Run(mainWindow);
                }
            }
            catch (Exception)
            {
                // Log the exception and exit
            }
        }
    }
}
