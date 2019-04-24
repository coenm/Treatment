namespace TestAgent
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Medallion.Shell;

    using ZeroMQ;

    public static class Program
    {
        private const string SutPublishPort = "5558";
        private const string SutReqRspPort = "5587";

#if DEBUG
        private const string CONFIG = "Debug";
#else
        private const string CONFIG = "Release";
#endif

        public static async Task Main(string[] args)
        {
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var treatmentDir = currentDir;
            while (!Directory.Exists(Path.Combine(treatmentDir, "src", "Treatment.UI.Start", "bin", "x64", CONFIG)))
            {
                treatmentDir = Path.GetFullPath(Path.Combine(treatmentDir, ".."));
            }

            treatmentDir = Path.GetFullPath(Path.Combine(treatmentDir, "src", "Treatment.UI.Start", "bin", "x64", CONFIG));
            var executable = Path.Combine(treatmentDir, "Treatment.UIStart.exe");

            if (!File.Exists(executable))
            {
                Console.WriteLine($"File {executable} doesn't exist.");
                Console.ReadLine();
                return;
            }

            var mreListening = new ManualResetEvent(false);
            var cts = new CancellationTokenSource();

            var context = new ZContext();

//            var task0 = Task.Run(() =>
//            {
//                using (var request = new ZSocket(context, ZSocketType.REQ))
//                using (cts.Token.Register(() => request.Dispose()))
//                {
//                    request.Connect($"tcp://localhost:{SutReqRspPort}");
//
//                    try
//                    {
//                        for (int i = 0; i < 10; i++)
//                        {
//                            var zmsg = new ZMessage();
//                            ZError error;
//
//                            request.SendMessage(new ZMessage()
//                            {
//                                new ZFrame("question " + i)
//                            });
//
//
//                            if (!request.ReceiveMessage(ref zmsg, ZSocketFlags.None, out error))
//                            {
//                                Console.WriteLine($" Oops, could not receive a request: {error}");
//                                return;
//                            }
//
//                            using (zmsg)
//                            {
//                                Console.WriteLine();
//                                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:fff"));
//                                Console.WriteLine("+" + new string('-', 120 + 2) + "+");
//
//                                foreach (var frame in zmsg)
//                                {
//                                    var s = frame.ReadString();
//                                    Console.WriteLine($"| {s,-120} |");
//                                }
//
//                                Console.WriteLine("+" + new string('-', 120 + 2) + "+");
//                                Console.WriteLine(" ");
//                            }
//                        }
//                    }
//                    catch (Exception e)
//                    {
//                        if (!cts.IsCancellationRequested)
//                            Console.WriteLine(e.Message);
//                    }
//                }
//            }).ConfigureAwait(false);


            var task = Task.Run(() =>
            {
                using (var subscriber = new ZSocket(context, ZSocketType.SUB))
                using (cts.Token.Register(() => subscriber.Dispose()))
                {
                    subscriber.Connect($"tcp://localhost:{SutPublishPort}");
                    subscriber.SubscribeAll();

                    try
                    {
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
                                    var m = string.Empty;
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
                    catch (Exception e)
                    {
                        if (!cts.IsCancellationRequested)
                            Console.WriteLine(e.Message);
                    }
                }
            }).ConfigureAwait(false);

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
                        new KeyValuePair<string, string>("TA_PUBLISH_SOCKET", $"tcp://*:{SutPublishPort}"), // sut publishes events on this
                        new KeyValuePair<string, string>("TA_REQ_RSP_SOCKET", $"tcp://*:{SutReqRspPort}"), // sut starts listening for requests on this port.
                    });
                });

            var result = await command.Task.ConfigureAwait(true);

            Console.WriteLine(result.StandardOutput);
            Console.WriteLine(result.StandardError);

            cts.Cancel();
            Console.WriteLine("Done. Press enter to exit.");
            Console.ReadLine();
            await task;
        }
    }
}
