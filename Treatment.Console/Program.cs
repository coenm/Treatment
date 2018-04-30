namespace Treatment.Console
{
    using System.Collections.Generic;

    using CommandLine;

    using SimpleInjector;

    using Treatment.Console.Console;
    using Treatment.Console.Options;
    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Contract.Queries;

    public static class Program
    {
        public static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<ListProvidersOptions, FixOptions>(args)
                         .MapResult(
                                    (ListProvidersOptions opts) => ListSearchProviders(opts),
                                    (FixOptions opts) => FixProjectFiles(opts),
                                    HoldConsoleOnError);
        }

        private static int HoldConsoleOnError(IEnumerable<Error> errs)
        {
            Bootstrap
                .Configure()
                .GetInstance<IConsole>()
                .ReadKey();

            return -1;
        }

        private static int FixProjectFiles(FixOptions options)
        {
            Bootstrap
                .Configure(options)
                .GetInstance<ICommandHandler<UpdateProjectFilesCommand>>()
                .Execute(new UpdateProjectFilesCommand(options.RootDirectory));

            return 0;
        }

        private static int ListSearchProviders(ListProvidersOptions options)
        {
            var container = new Container();
            Bootstrapper2.Bootstrap(container);
            using (Bootstrapper2.StartSession(container))
            {
                var result = Bootstrapper2.ExecuteQuery(new GetAllSearchProvidersQuery(), container);

                var console = container.GetInstance<IConsole>();

                console.WriteLine("Installed search providers (ordered by priority):");
                foreach (var f in result)
                    console.WriteLine($"- {f.Name}");
            }

            return 0;
        }
    }
}
