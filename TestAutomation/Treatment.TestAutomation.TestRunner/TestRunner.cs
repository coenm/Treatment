namespace Treatment.TestAutomation.TestRunner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using global::TestAutomation.Contract.Input.Interface.Base;
    using global::TestAutomation.Contract.Input.Interface.Input.Mouse;
    using global::TestAutomation.Contract.Input.Serializer;
    using SimpleInjector;

    using TestAgent.Contract.Interface.Control;
    using TestAgent.Contract.Serializer;
    using TreatmentZeroMq.ContextService;
    using TreatmentZeroMq.Helpers;
    using Xunit;
    using ZeroMQ;

    public class TestRunner : IDisposable
    {
        private Container container;
        private ManualResetEvent mreListening;
        private CancellationTokenSource cts;
        private ZContext context;
        private ZSocket socket;
        private bool connected;

        public TestRunner()
        {
            container = new Container();
            container.RegisterSingleton<IZeroMqContextService, ZeroMqContextService>();

            context = container.GetInstance<IZeroMqContextService>().GetContext();

            socket = new ZSocket(context, ZSocketType.REQ) { Linger = TimeSpan.Zero };
            connected = socket.TryConnect($"tcp://localhost:{Settings.AgentReqRspPort}");
            ZmqConnection.GiveZeroMqTimeToFinishConnectOrBind();

            mreListening = new ManualResetEvent(false);

            cts = new CancellationTokenSource();
        }

        public void Dispose()
        {
            socket.Dispose();
            container.GetInstance<IZeroMqContextService>().DisposeCurrentContext();
            container?.Dispose();
        }

        [Fact]
        public void StartSutTest()
        {
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var slnDir = currentDir;
            while (!File.Exists(Path.Combine(slnDir, "Treatment.sln")) && slnDir?.Length > 4)
            {
                slnDir = Path.GetFullPath(Path.Combine(slnDir, ".."));
            }

            var (type, payload) = TestAgentRequestResponseSerializer.Serialize(new LocateFilesRequest { Directory = slnDir, Filename = "Treatment.UIStart.exe", });

            var foundExecutable = string.Empty;

            var msg = new ZMessage(new List<ZFrame> { new ZFrame("TESTAGENT"), new ZFrame(type), new ZFrame(payload), });
            if (socket.TrySend(msg, 5, i => 10 * i))
            {
                if (socket.TryReceive(out var rsp, 5, i => i * 10))
                {
                    if (rsp.Count >= 2)
                    {
                        var t = rsp.Pop().ReadString();
                        var p = rsp.Pop().ReadString();
                        var resp = TestAgentRequestResponseSerializer.DeserializeResponse(t, p);
                        if (resp is LocateFilesResponse locateFilesRsp)
                        {
                            foreach (var e in locateFilesRsp.Executable)
                            {
                                Console.WriteLine($"| {e,-100} |");
                            }

                            foundExecutable = locateFilesRsp.Executable.FirstOrDefault(x => x.EndsWith("Treatment.UI.Start\\bin\\x64\\Debug\\Treatment.UIStart.exe"));
                            if (string.IsNullOrWhiteSpace(foundExecutable))
                            {
                                foundExecutable = locateFilesRsp.Executable.FirstOrDefault();
                            }

                            if (string.IsNullOrWhiteSpace(foundExecutable))
                            {
                                Console.WriteLine("NOTHING FOUND");
                            }
                            else
                            {
                                Console.WriteLine($">>> {foundExecutable}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"| {t,-100} |");
                            Console.WriteLine($"| {p,-100} |");
                        }
                    }
                    else
                    {
                        foreach (var frame in rsp)
                        {
                            var s = frame.ReadString();
                            Console.WriteLine($"| {s,-100} |");
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(foundExecutable))
            {
                (type, payload) = TestAgentRequestResponseSerializer.Serialize(new StartSutRequest
                {
                    Executable = foundExecutable,
                    WorkingDirectory = new FileInfo(foundExecutable).DirectoryName,
                });

                msg = new ZMessage(new List<ZFrame> { new ZFrame("TESTAGENT"), new ZFrame(type), new ZFrame(payload), });
                if (socket.TrySend(msg, 5, i => 10 * i))
                {
                    if (socket.TryReceive(out var rsp, 5, i => i * 10))
                    {
                        if (rsp.Count >= 2)
                        {
                            var t = rsp.Pop().ReadString();
                            var p = rsp.Pop().ReadString();
                            var resp = TestAgentRequestResponseSerializer.DeserializeResponse(t, p);

                            Console.WriteLine($"| {t,-100} |");
                            Console.WriteLine($"| {p,-100} |");
                        }
                        else
                        {
                            foreach (var frame in rsp)
                            {
                                var s = frame.ReadString();
                                Console.WriteLine($"| {s,-100} |");
                            }
                        }
                    }
                }
            }
        }

        [Fact]
        public void ClickMouse()
        {
            Thread.Sleep(5000);

            int x = 20;
            int y = 60;

//            for (x = 0; x < 1920; x += 100)
//            {
//                for (y = 0; y < 1080; y += 100)
//                {
                    var pos = new Point { X = x, Y = y };
                    var (type, payload) = RequestResponseSerializer.Serialize(new MoveMouseToRequest { Position = pos });

                    var msg = new ZMessage(new List<ZFrame> { new ZFrame("SUT"), new ZFrame(type), new ZFrame(payload), });

                    if (socket.TrySend(msg))
                    {
                        if (socket.TryReceive(out var rsp))
                        {
                            foreach (var frame in rsp)
                            {
                                var s = frame.ReadString();
                                Console.WriteLine($"| {s,-100} |");
                            }
                        }
                    }

                    Thread.Sleep(1000);
//                }
//            }
        }
    }
}
