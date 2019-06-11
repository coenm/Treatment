namespace Treatment.UI.Core
{
    using Core.Configuration;
    using Helpers.Guards;
    using Implementations.Configuration;
    using Implementations.Delay;
    using JetBrains.Annotations;
    using Model;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using Treatment.Core.Bootstrap;
    using Treatment.Core.DefaultPluginImplementation.FileSearch;
    using View;
    using ViewModel;
    using Wpf.Framework.Application;
    using Wpf.Framework.EntityEditor;
    using Wpf.Framework.EntityEditor.View;
    using Wpf.Framework.EntityEditor.ViewModel;
    using Wpf.Framework.SynchronizationContext;

    [PublicAPI]
    public static class UiBootstrapper
    {
        [PublicAPI]
        public static void Bootstrap([NotNull] Container container)
        {
            Guard.NotNull(container, nameof(container));

            CoreBootstrap.Bootstrap(container);

            // container.Register<IGetActivatedWindow, PInvokeActivatedWindow>(Lifestyle.Singleton);
            container.Register<IGetActivatedWindow, ApplicationActivatedWindow>(Lifestyle.Singleton);

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

            RegisterUserInterfaceDependencies(container);

            RegisterDelay(container);


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


    }
}
