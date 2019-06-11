namespace Treatment.UI
{
    using System;
    using System.IO;
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
    using Treatment.UI.Implementations.Configuration;
    using Treatment.UI.Implementations.Delay;
    using Treatment.UI.Model;
    using Treatment.UI.View;
    using Treatment.UI.ViewModel;
    using Wpf.Framework.Application;
    using Wpf.Framework.EntityEditor;
    using Wpf.Framework.EntityEditor.View;
    using Wpf.Framework.EntityEditor.ViewModel;
    using Wpf.Framework.SynchronizationContext;

    [PublicAPI]
    public static class Bootstrapper
    {
        [PublicAPI]
        public static void Bootstrap([NotNull] Container container)
        {
            Guard.NotNull(container, nameof(container));

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Options.AllowOverridingRegistrations = true;

            CoreBootstrap.Bootstrap(container);

            container.Register<IGetActivatedWindow, ApplicationActivatedWindow>(Lifestyle.Singleton);
            // container.Register<ICurrentWindow, PInvokeActivatedWindow>(Lifestyle.Singleton);

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

            container.RegisterSingleton<IEditorByTypeFactory, SimpleInjectorEditorByTypeFactory>();
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
    }
}
