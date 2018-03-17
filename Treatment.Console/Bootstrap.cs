namespace Treatment.Console
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

    using SimpleInjector;

    using Treatment.Console.Options;
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
        public static Container Configure([NotNull] Options.Options opts)
        {
            var verbose = false;
            var summary = false;
            var dryRun = false;
            var rootDirectory = string.Empty;
            var searchProvider = string.Empty;
            var holdOnExit = opts.HoldOnExit;

            if (opts is FixOptions fixOptions)
            {
                verbose = fixOptions.Verbose;
                dryRun = fixOptions.DryRun;
                summary = fixOptions.Summary;
                searchProvider = fixOptions.SearchProvider;
                rootDirectory = fixOptions.RootDirectory;
            }

            return Configure(verbose, summary, holdOnExit, dryRun, rootDirectory, searchProvider);
        }

        private static Container Configure(bool verbose, bool summary, bool holdOnExit, bool dryRun, string rootDirectory, string searchProviderName)
        {
            var container = new Container();

            // Plugins might register more Search Provider Factories
            container.RegisterCollection<ISearchProviderFactory>(new[] { typeof(OsFileSystemSearchProviderFactory) });

            container.RegisterSingleton<IFileSystem>(OsFileSystem.Instance);
            container.Register(() => CreateSearchProvider(container, searchProviderName), Lifestyle.Singleton);

            var registrationStatisticsCollectorAndSummaryWriter = Lifestyle.Singleton.CreateRegistration<StatisticsCollectorAndSummaryWriter>(container);
            container.AddRegistration(typeof(IStatistics), registrationStatisticsCollectorAndSummaryWriter);

            // Register all commandhandlers found in the specific assembly.
            container.Register(typeof(ICommandHandler<>), new[] { typeof(ICommandHandler<>).Assembly });

            container.RegisterSingleton<IRootDirSanitizer>(() => new RemoveRootDirSanitizer(rootDirectory));

            if (dryRun)
            {
                // Decorate IFileSystem not to save data to disk
                container.RegisterDecorator<IFileSystem, DryRunFileSystemDecorator>();
            }

            if (verbose)
            {
                container.RegisterDecorator<IFileSystem, VerboseFileSystemDecorator>();
                container.RegisterDecorator<IFileSearch, VerboseFileSearchDecorator>();
            }

            if (summary)
            {
                container.AddRegistration(typeof(ISummaryWriter), registrationStatisticsCollectorAndSummaryWriter);

                container.RegisterDecorator<IFileSystem, SummaryCollectorFileSystemDecorator>();
                container.RegisterDecorator<IFileSearch, SummaryCollectorFileSearchDecorator>();

                // Add summary decorator for the UpdateProjectFilesCommand-CommandHandler.
                container.RegisterDecorator(
                                            typeof(ICommandHandler<>),
                                            typeof(SummaryDecorator<>),
                                            context => typeof(UpdateProjectFilesCommand).IsAssignableFrom(context.ServiceType.GetGenericArguments()[0]));
            }
            else
            {
                container.RegisterSingleton<ISummaryWriter, FakeSummaryWriter>();
            }

            if (holdOnExit)
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

        private static IFileSearch CreateSearchProvider([NotNull] Container container, [NotNull] string searchProvider)
        {
            var factories = container.GetAllInstances<ISearchProviderFactory>();

            var factory = factories
                          .OrderBy(f => f.Priority)
                          .FirstOrDefault(item => item.CanCreate(searchProvider));

            if (factory == null)
                return OsFileSystem.Instance;

            return factory.Create();
        }
    }
}