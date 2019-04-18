using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestAgent
{
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using Medallion.Shell;
    using ZeroMQ;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var treatmentDir = Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", "..", "src", "Treatment.UI.Start", "bin", "x64", "Debug"));
            var executable = Path.Combine(treatmentDir, "Treatment.UIStart.exe");

            if (!File.Exists(executable))
            {
                Console.WriteLine($"File {executable} doesn't exist.");
                Console.ReadLine();
                return;
            }

            var mreListening = new ManualResetEvent(false);
            var cts = new CancellationTokenSource();

            Task.Run(() =>
            {
                using (var context = new ZContext())
                using (var subscriber = new ZSocket(context, ZSocketType.SUB))
                using (cts.Token.Register(() => context.Dispose()))
                {
                    subscriber.Connect("tcp://localhost:5556");
                    subscriber.SubscribeAll();

                    while (true)
                    {
                        var zmsg = new ZMessage();
                        ZError error;
                        mreListening.Set();
                        if (!subscriber.ReceiveMessage(ref zmsg, ZSocketFlags.None, out error))
                        {
                            Console.WriteLine($" Oops, could not receive a request: {error}");
                            return;
                        }

                        using (zmsg)
                        using (var zmsg2 = zmsg.Duplicate())
                        {
                            Console.WriteLine();
                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:fff"));
                            Console.WriteLine("+" + new string('-', 100 + 2) + "+");

                            bool subscribeUnsubscribe = false;
                            if (zmsg2.Count == 1 && zmsg2[0].Length == 1)
                            {
                                var b = zmsg2.PopAsByte();
                                var m = "";
                                if (b == 0x01)
                                    m = "SUBSCRIBE  (0x01)";
                                if (b == 0x00)
                                    m = "UNSUBSCRIBE (0x00)";

                                if (!string.IsNullOrEmpty(m))
                                {
                                    subscribeUnsubscribe = true;
                                    Console.WriteLine($"| {m,-100} |");
                                }
                            }

                            if (!subscribeUnsubscribe)
                            {
                                foreach (var frame in zmsg)
                                {
                                    var s = frame.ReadString();
                                    Console.WriteLine($"| {s,-100} |");
                                }
                            }

                            Console.WriteLine("+" + new string('-', 100 + 2) + "+");
                            Console.WriteLine(" ");
                        }
                    }
                }
            });

            mreListening.WaitOne();

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
                        new KeyValuePair<string, string>("TA_PUBLISH_SOCKET", "tcp://*:5556"), // sut publishes events on this
                        new KeyValuePair<string, string>("TA_REQ_RSP_SOCKET", "tcp://*:5557"), // sut starts listening for requests on this port.
                    });
                });

            var result = command.Task.GetAwaiter().GetResult();

            Console.WriteLine(result.StandardOutput);
            Console.WriteLine(result.StandardError);

            Console.WriteLine("done");
            Console.ReadKey();
            cts.Cancel();
        }
    }
}
