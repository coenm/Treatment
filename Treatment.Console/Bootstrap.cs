namespace Treatment.Console
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

    using SimpleInjector;

    using Treatment.Core;
    using Treatment.Core.FileSearch;
    using Treatment.Core.FileSystem;
    using Treatment.Core.Interfaces;
    using Treatment.Core.Statistics;
    using Treatment.Core.UseCases;
    using Treatment.Core.UseCases.Decorators;
    using Treatment.Core.UseCases.UpdateProjectFiles;

    public static class Bootstrap
    {
        public static Container Configure([NotNull] Options opts)
        {
            var container = new Container();

            // not sure if this is needed.
            container.Options.AllowOverridingRegistrations = true;

            // Plugins might register more Search Provider Factories
            container.RegisterCollection<ISearchProviderFactory>(new[] { typeof(OsFileSystemSearchProviderFactory) });

            container.RegisterSingleton<IFileSystem>(OsFileSystem.Instance);

            container.Register(() => CreateSearchProvider(container, opts), Lifestyle.Singleton);

            container.RegisterSingleton<IRootDirSanitizer>(() => new RemoveRootDirSanitizer(opts.RootDirectory));

            var registrationStatisticsCollectorAndSummaryWriter = Lifestyle.Singleton.CreateRegistration<StatisticsCollectorAndSummaryWriter>(container);
            container.AddRegistration(typeof(IStatistics), registrationStatisticsCollectorAndSummaryWriter);

            if (opts.Summary)
                container.AddRegistration(typeof(ISummaryWriter), registrationStatisticsCollectorAndSummaryWriter);
            else
                container.RegisterSingleton<ISummaryWriter, FakeSummaryWriter>();

            // decorate registrations based on given options.
            if (opts.DryRun)
                container.RegisterDecorator<IFileSystem, DryRunFileSystemDecorator>();

            if (opts.Verbose)
            {
                container.RegisterDecorator<IFileSystem, VerboseFileSystemDecorator>();
                container.RegisterDecorator<IFileSearch, VerboseFileSearchDecorator>();
            }

            if (opts.Summary)
            {
                container.RegisterDecorator<IFileSystem, SummaryFileSystemDecorator>();
                container.RegisterDecorator<IFileSearch, SummaryFileSearchDecorator>();
            }

            // register all commandhandlers found in the specific assembly.
            container.Register(typeof(ICommandHandler<>), new[] { typeof(ICommandHandler<>).Assembly });

            if (opts.Summary)
            {
                // Add summary decorator for the UpdateProjectFilesCommand-CommandHandler.
                container.RegisterDecorator(
                                            typeof(ICommandHandler<>),
                                            typeof(SummaryDecorator<>),
                                            context => typeof(UpdateProjectFilesCommand).IsAssignableFrom(context.ServiceType.GetGenericArguments()[0]));
            }

            if (opts.HoldOnExit)
            {
                container.RegisterDecorator(typeof(ICommandHandler<>), typeof(HoldConsoleDecorator<>));
            }

            RegisterPlugins(container);

            container.Verify();

            return container;
        }

        private static void RegisterPlugins([NotNull] Container container)
        {
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

        private static IFileSearch CreateSearchProvider([NotNull] Container container, [NotNull] Options opts)
        {
            var factories = container.GetAllInstances<ISearchProviderFactory>();

            var factory = factories
                          .OrderBy(f => f.Priority)
                          .FirstOrDefault(item => item.CanCreate(opts.SearchProvider));

            if (factory == null)
                return OsFileSystem.Instance;

            return factory.Create();
        }
    }
}