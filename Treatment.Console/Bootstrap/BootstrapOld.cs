namespace Treatment.Console.Bootstrap
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using FluentValidation;

    using JetBrains.Annotations;

    using SimpleInjector;

    using Treatment.Console.CommandLineOptions;
    using Treatment.Console.Console;
    using Treatment.Console.CrossCuttingConcerns;
    using Treatment.Console.Decorators;
    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Core.DefaultPluginImplementation.FileSearch;
    using Treatment.Core.FileSearch;
    using Treatment.Core.FileSystem;
    using Treatment.Core.Interfaces;
    using Treatment.Core.Statistics;
    using Treatment.Core.UseCases.CrossCuttingConcerns;
    using Treatment.Core.UseCases.UpdateProjectFiles;

    internal static class BootstrapOld
    {
        public static Container Configure([CanBeNull] Options opts = null)
        {
            var verbose = false;
            var summary = false;
            var dryRun = false;
            var rootDirectory = string.Empty;
            var searchProvider = string.Empty;
            var holdOnExit = false;

            if (opts != null)
            {
                holdOnExit = opts.HoldOnExit;
            }

            if (opts is FixOptions fixOptions)
            {
                verbose = fixOptions.Verbose > 0;
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

            container.RegisterInstance<IConsole>(ConsoleAdapter.Instance);

            // Plugins might register more Search Provider Factories
            container.RegisterCollection<ISearchProviderFactory>(new[] { typeof(OsFileSystemSearchProviderFactory) });

            RegisterFluentValidationValidators(container);

            container.RegisterInstance<IFileSystem>(OsFileSystem.Instance);
            container.Register(() => CreateSearchProvider(container, searchProviderName), Lifestyle.Singleton);

            var registrationStatisticsCollectorAndSummaryWriter = Lifestyle.Singleton.CreateRegistration<StatisticsCollectorAndSummaryWriter>(container);
            container.AddRegistration(typeof(IStatistics), registrationStatisticsCollectorAndSummaryWriter);

            // Register all commandhandlers found in the specific assembly.
            container.Register(typeof(ICommandHandler<>), new[] { typeof(ICommandHandler<>).Assembly });


            // container.RegisterSingleton<IQueryProcessor>(new DynamicQueryProcessor(container));

            container.RegisterSingleton<IRootDirSanitizer>(() =>
                                                           {
                                                               var result = new RemoveRootDirSanitizer();
                                                               result.SetRootDir(rootDirectory);
                                                               return result;
                                                           });

            if (dryRun)
            {
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

            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(CommandValidationDecorator<>));

            if (holdOnExit)
            {
                container.RegisterDecorator(typeof(ICommandHandler<>), typeof(HoldConsoleCommandHandlerDecorator<>));
            }

            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(WriteExceptionToConsoleCommandHandlerDecorator<>));


            container.RegisterSingleton<IHoldConsole, HoldConsole>();


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

        private static void RegisterFluentValidationValidators(Container container)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            container.Register(typeof(IValidator<>), assemblies);
        }

        private static IFileSearch CreateSearchProvider([NotNull] Container container, [NotNull] string searchProvider)
        {
            var factories = container.GetAllInstances<ISearchProviderFactory>();

            var factory = factories
                          .OrderBy(f => f.Priority)
                          .FirstOrDefault(item => item.CanCreate(searchProvider));

            if (factory == null)
                return OsFileSearch.Instance;

            return factory.Create();
        }
    }
}