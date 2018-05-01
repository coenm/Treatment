﻿namespace Treatment.Console
{
    using System.Collections.Generic;

    using CommandLine;

    using SimpleInjector;

    using Treatment.Console.Bootstrap;
    using Treatment.Console.CommandLineOptions;
    using Treatment.Console.Console;
    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Contract.Queries;
    using Treatment.Core.DefaultPluginImplementation.FileSearch;

    public static class Program
    {
        // ReSharper disable once MemberCanBePrivate.Global
        internal static Bootstrapper Bootstrapper { get; set; }

        public static int Main(string[] args)
        {
            if (Bootstrapper == null)
                Bootstrapper = new Bootstrapper();

            Bootstrapper.Init();
            Bootstrapper.RegisterDefaultOptions();

            return Parser.Default.ParseArguments<ListProvidersOptions, FixOptions>(args)
                         .MapResult(
                                    (ListProvidersOptions opts) => ListSearchProviders(opts),
                                    (FixOptions opts) => FixProjectFiles(opts),
                                    HoldConsoleOnError);
        }

        private static int HoldConsoleOnError(IEnumerable<Error> errs)
        {
            Bootstrapper
                .Container
                .GetInstance<IConsole>()
                .ReadKey();

            return -1;
        }

        private static int FixProjectFiles(FixOptions options)
        {
            var staticOptions = new StaticOptions(
                                                  options.Verbose ? VerboseLevel.High : VerboseLevel.Null,
                                                  options.DryRun,
                                                  options.HoldOnExit,
                                                  options.SearchProvider);

            Bootstrapper.Container.Register(typeof(IHoldOnExitOption), () => staticOptions, Lifestyle.Scoped);
            Bootstrapper.Container.Register(typeof(IDryRunOption), () => staticOptions, Lifestyle.Scoped);
            Bootstrapper.Container.Register(typeof(ISearchProviderNameOption), () => staticOptions, Lifestyle.Scoped);
            Bootstrapper.Container.Register(typeof(IVerboseOption), () => staticOptions, Lifestyle.Scoped);

            // Bootstrapper.VerifyContainer();

            using (Bootstrapper.StartSession())
            {
                var commandHandler = Bootstrapper.Container.GetInstance<ICommandHandler<UpdateProjectFilesCommand>>();
                commandHandler.Execute(new UpdateProjectFilesCommand(options.RootDirectory));
            }

            return 0;
        }

        private static int ListSearchProviders(ListProvidersOptions options)
        {
            Bootstrapper.Container.Register(typeof(IHoldOnExitOption),
                                            () => new StaticOptions(VerboseLevel.Null, false, options.HoldOnExit, string.Empty),
                                            Lifestyle.Scoped);

            Bootstrapper.VerifyContainer();

            using (Bootstrapper.StartSession())
            {
                var result = Bootstrapper.ExecuteQuery(new GetAllSearchProvidersQuery());

                var console = Bootstrapper.Container.GetInstance<IConsole>();

                console.WriteLine("Installed search providers (ordered by priority):");
                foreach (var f in result)
                    console.WriteLine($"- {f.Name}");

                if (options.HoldOnExit)
                    console.ReadLine();
            }

            return 0;
        }
    }
}
