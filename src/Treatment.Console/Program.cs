namespace Treatment.Console
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CommandLine;

    using Nito.AsyncEx;

    using SimpleInjector;

    using Treatment.Console.Bootstrap;
    using Treatment.Console.CommandLineOptions;
    using Treatment.Console.Console;
    using Treatment.Console.CrossCuttingConcerns;
    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Contract.Queries;
    using Treatment.Core.DefaultPluginImplementation.FileSearch;
    using Treatment.Core.DefaultPluginImplementation.SourceControl;

    public static class Program
    {
        // ReSharper disable once MemberCanBePrivate.Global
        internal static Bootstrapper Bootstrapper { get; set; }

        public static int Main(params string[] args)
        {
            return AsyncContext.Run(() => MainAsync(args));
        }

        public static async Task<int> MainAsync(params string[] args)
        {
            if (Bootstrapper == null)
                Bootstrapper = new Bootstrapper();

            Bootstrapper.Init();
            Bootstrapper.RegisterDefaultOptions();

            var result = Parser.Default.ParseArguments<RemoveNewAppConfigOptions, ListProvidersOptions, FixOptions>(args);
            if (result == null)
                throw new Exception("no result");

            if (result is Parsed<object> parsed)
            {
                switch (parsed.Value)
                {
                    case RemoveNewAppConfigOptions removeNewAppConfigOptions:
                        return await RemoveNewAppConfigAsync(removeNewAppConfigOptions).ConfigureAwait(true);

                    case FixOptions fixOptions:
                        return await FixProjectFilesAsync(fixOptions).ConfigureAwait(true);

                    case ListProvidersOptions listProvidersOptions:
                        return await ListProvidersAsync(listProvidersOptions).ConfigureAwait(true);

                    default:
                        throw new NotImplementedException();
                }
            }

            return HoldConsoleOnError(((NotParsed<object>)result).Errors);
        }

        private static int HoldConsoleOnError(IEnumerable<Error> errs)
        {
            using (Bootstrapper.StartSession())
            {
                Bootstrapper
                    .Container
                    .GetInstance<IConsole>()
                    .ReadKey();
            }

            return -1;
        }

        private static async Task<int> RemoveNewAppConfigAsync(RemoveNewAppConfigOptions options)
        {
            VerboseLevel Map(int value)
            {
                switch (value)
                {
                    case 3:
                        return VerboseLevel.High;
                    case 2:
                        return VerboseLevel.Medium;
                    case 1:
                        return VerboseLevel.Low;
                    case 0:
                        return VerboseLevel.Disabled;
                }
                return VerboseLevel.Disabled;
            }

            var staticOptions = new StaticOptions(
                                                  Map(options.Verbose),
                                                  options.DryRun,
                                                  options.HoldOnExit,
                                                  options.SearchProvider,
                                                  options.SourceControlProvider);

            Bootstrapper.Container.Register(typeof(IHoldOnExitOption), () => staticOptions, Lifestyle.Scoped);
            Bootstrapper.Container.Register(typeof(IDryRunOption), () => staticOptions, Lifestyle.Scoped);
            Bootstrapper.Container.Register(typeof(ISearchProviderNameOption), () => staticOptions, Lifestyle.Scoped);
            Bootstrapper.Container.Register(typeof(IVerboseOption), () => staticOptions, Lifestyle.Scoped);
            Bootstrapper.Container.Register(typeof(ISourceControlNameOption), () => staticOptions, Lifestyle.Scoped);

            using (Bootstrapper.StartSession())
            {
                var commandHandler = Bootstrapper.Container.GetInstance<ICommandHandler<CleanAppConfigCommand>>();
                await commandHandler.ExecuteAsync(new CleanAppConfigCommand(options.RootDirectory)).ConfigureAwait(true);
            }

            return 0;
        }
        private static async Task<int> FixProjectFilesAsync(FixOptions options)
        {
            VerboseLevel Map(int value)
            {
                switch (value)
                {
                    case 3:
                        return VerboseLevel.High;
                    case 2:
                        return VerboseLevel.Medium;
                    case 1:
                        return VerboseLevel.Low;
                    case 0:
                        return VerboseLevel.Disabled;
                }
                return VerboseLevel.Disabled;
            }

            var staticOptions = new StaticOptions(
                                                  Map(options.Verbose),
                                                  options.DryRun,
                                                  options.HoldOnExit,
                                                  options.SearchProvider,
                                                  string.Empty);

            Bootstrapper.Container.Register(typeof(IHoldOnExitOption), () => staticOptions, Lifestyle.Scoped);
            Bootstrapper.Container.Register(typeof(IDryRunOption), () => staticOptions, Lifestyle.Scoped);
            Bootstrapper.Container.Register(typeof(ISearchProviderNameOption), () => staticOptions, Lifestyle.Scoped);
            Bootstrapper.Container.Register(typeof(IVerboseOption), () => staticOptions, Lifestyle.Scoped);

            using (Bootstrapper.StartSession())
            {
                var commandHandler = Bootstrapper.Container.GetInstance<ICommandHandler<UpdateProjectFilesCommand>>();
                await commandHandler.ExecuteAsync(new UpdateProjectFilesCommand(options.RootDirectory)).ConfigureAwait(true);
            }

            return 0;
        }

        private static Task<int> ListProvidersAsync(ListProvidersOptions options)
        {
            Bootstrapper.Container.Register(
                                            typeof(IHoldOnExitOption),
                                            () => new StaticOptions(VerboseLevel.Disabled, false, options.HoldOnExit, string.Empty, string.Empty),
                                            Lifestyle.Scoped);

            using (Bootstrapper.StartSession())
            {
                var console = Bootstrapper.Container.GetInstance<IConsole>();

                var searchProviders = Bootstrapper.ExecuteQuery(new GetAllSearchProvidersQuery());
                console.WriteLine("Installed search providers (ordered by priority):");
                foreach (var f in searchProviders)
                    console.WriteLine($"- {f.Name}");


                System.Console.WriteLine();

                var versionControlProviders = Bootstrapper.ExecuteQuery(new GetAllVersionControlProvidersQuery());
                console.WriteLine("Installed version control providers (ordered by priority):");
                foreach (var f in versionControlProviders)
                    console.WriteLine($"- {f.Name}");


                if (options.HoldOnExit)
                {
                    var holdOnConsole = Bootstrapper.Container.GetInstance<IHoldConsole>();
                    holdOnConsole.Hold();
                }
            }

            return Task.FromResult(0);
        }
    }
}
