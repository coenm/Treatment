namespace Treatment.Console
{
    using System;
    using System.Linq;

    using Treatment.Core;
    using Treatment.Core.Interfaces;

    public class Execute
    {
        public void Go(Options options)
        {
            var container = Bootstrap.Configure(options);

            Console.WriteLine($"Processing directory: '{options.RootDirectory}'");
            var fixer = container.GetInstance<RelativePathInCsProjFixer>();

            string[] csFiles;
            try
            {
                csFiles = fixer.GetCsFiles(options.RootDirectory);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Auch... Cannot read {options.RootDirectory} ?!");
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

            if (!options.HoldOnExit)
                return;

            Console.WriteLine("Done. Press a key to exit.");
            Console.ReadKey();
        }
    }
}