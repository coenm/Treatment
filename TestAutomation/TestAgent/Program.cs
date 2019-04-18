using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestAgent
{
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using Medallion.Shell;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var treatmentDir = Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", "..", "src", "Treatment.UI.Start", "bin", "x64", "Debug"));
            var executable = Path.Combine(treatmentDir, "Treatment.UIStart.exe");

            if (!File.Exists(executable))
            {
                Console.WriteLine($"File {executable} doesnt exist.");
                Console.ReadLine();
                return;
            }

            var cts = new CancellationTokenSource(60000);
            var command = Command.Run(
                executable,
                new string[0],
                options =>
                {
                    options.WorkingDirectory(treatmentDir);
                    options.CancellationToken(cts.Token);
                    options.EnvironmentVariables(new[]
                    {
                        new KeyValuePair<string, string>("ENABLE_TEST_AUTOMATION", "true"),
                        new KeyValuePair<string, string>("TA_KEY", string.Empty),
                        new KeyValuePair<string, string>("TA_PUBLISH_SOCKET", "tpc://*:123457"), // sut publishes events on this
                        new KeyValuePair<string, string>("TA_REQ_RSP_SOCKET", "tpc://*:123458"), // sut starts listening for requests on this port.
                    });
                });

            var result = command.Task.GetAwaiter().GetResult();

            Console.WriteLine(result.StandardOutput);
            Console.WriteLine(result.StandardError);

            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
