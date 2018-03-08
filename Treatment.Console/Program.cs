namespace Treatment.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandLine;

    using Treatment.Core;
    using Treatment.Core.Interfaces;

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

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("Could not parse arguments.");
            foreach (var item in errs)
                Console.WriteLine($" - {item}");

            Console.WriteLine(string.Empty);
            Console.WriteLine("Press key to exit");
            Console.ReadKey();
        }

        private static void RunOptionsAndReturnExitCode(Options opts)
        {
            var container = Bootstrap.Configure(opts);

            Console.WriteLine($"Processing directory: '{opts.RootDirectory}'");
            var fixer = container.GetInstance<RelativePathInCsProjFixer>();

            string[] csFiles;
            try
            {
                csFiles = fixer.GetCsFiles(opts.RootDirectory);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Auch... Cannot read {opts.RootDirectory} ?!");
                Console.WriteLine($"{e.Message}");
                Console.WriteLine(string.Empty);
                Console.WriteLine("Press a key to exit.");
                Console.ReadKey();
                return;
            }

            if (!csFiles.Any())
            {
                Console.WriteLine("Hmmm..  nothing found");
            }

            foreach (var file in csFiles)
            {
                try
                {
                    fixer.FixSingleFile(file);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            var summaryWriter = container.GetInstance<ISummaryWriter>();
            summaryWriter.OutputSummary();

            if (!opts.HoldOnExit)
                return;

            Console.WriteLine("Done. Press a key to exit.");
            Console.ReadKey();
        }
    }
}
