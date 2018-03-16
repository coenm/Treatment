namespace Treatment.Console
{
    using System.Collections.Generic;

    using CommandLine;

    using Treatment.Core.UseCases;
    using Treatment.Core.UseCases.ListSearchProviders;
    using Treatment.Core.UseCases.UpdateProjectFiles;

    using Console = System.Console;

    public static class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default
                  .ParseArguments<Options>(args)
                  .WithParsed(RunOptionsAndReturnExitCode)
                  .WithNotParsed(HandleParseError);
        }

        private static void RunOptionsAndReturnExitCode(Options options)
        {
            var container = Bootstrap.Configure(options);

            if (options.ListProviders)
            {
                container.GetInstance<ICommandHandler<ListSearchProvidersCommand>>()
                         .Execute(new ListSearchProvidersCommand());
            }
            else
            {
                container.GetInstance<ICommandHandler<UpdateProjectFilesCommand>>()
                         .Execute(new UpdateProjectFilesCommand(options.RootDirectory));
            }
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("Could not parse arguments.");
            foreach (var item in errs)
                Console.WriteLine($" - {item}");

            Console.WriteLine(string.Empty);
            Console.WriteLine("Press key to exit");
            Console.ReadKey();
        }
    }
}
