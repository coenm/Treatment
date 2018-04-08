namespace Treatment.Console
{
    using System.Collections.Generic;

    using CommandLine;

    using Treatment.Console.Console;
    using Treatment.Console.Options;
    using Treatment.Core.UseCases;
    using Treatment.Core.UseCases.ListSearchProviders;
    using Treatment.Core.UseCases.UpdateProjectFiles;

    using Console = System.Console;

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
            Bootstrap
                .Configure(options)
                .GetInstance<ICommandHandler<ListSearchProvidersCommand>>()
                .Execute(new ListSearchProvidersCommand());

            return 0;
        }
    }
}
