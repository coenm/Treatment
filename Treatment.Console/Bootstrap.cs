namespace Treatment.Console
{
    using JetBrains.Annotations;

    using SimpleInjector;

    using Treatment.Core;
    using Treatment.Core.FileSearch;
    using Treatment.Core.FileSystem;
    using Treatment.Core.Interfaces;
    using Treatment.Core.Statistics;
    using Treatment.Everything;

    public static class Bootstrap
    {
        public static Container Configure([NotNull] Options opts)
        {
            var container = new Container();

            container.RegisterSingleton<IFileSystem>(OsFileSystem.Instance);
            container.Register(() => CreateSearchProvider(opts), Lifestyle.Singleton);
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

            container.Verify();

            return container;
        }

        private static IFileSearch CreateSearchProvider(Options opts)
        {
            if (opts.SearchProvider == SearchProvider.Everything)
                return new EverythingFileSeachAdapter();

            return OsFileSystem.Instance;
        }
    }
}