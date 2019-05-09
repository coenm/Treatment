namespace Treatment.TestAutomation.TestRunner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Treatment.TestAutomation.TestRunner.Framework;
    using global::TestAutomation.Input.Contract.Interface.Base;
    using global::TestAutomation.Input.Contract.Interface.Input.Mouse;
    using global::TestAutomation.Input.Contract.Serializer;
    using SimpleInjector;

    using TestAgent.Contract.Interface.Control;
    using TestAgent.Contract.Serializer;

    using Treatment.TestAutomation.TestRunner.Helpers;

    using TreatmentZeroMq.ContextService;
    using TreatmentZeroMq.Helpers;
    using TreatmentZeroMq.Socket;
    using Xunit;
    using Xunit.Abstractions;
    using ZeroMQ;

    public class TestRunner : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly Container container;
        private readonly ZSocket socket;
        private bool connected;

        public TestRunner(ITestOutputHelper output)
        {
            this.output = output;

            container = new Bootstrapper().RegisterAll();

            var socketFactory = container.GetInstance<IZeroMqSocketFactory>();
            socket = socketFactory.Create(ZSocketType.REQ);

            connected = socket.TryConnect($"tcp://localhost:{Settings.AgentReqRspPort}");

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
            var slnDir = FileSystem.GetSolutionDirectory();
            slnDir.Should().NotBeNullOrEmpty();

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
                                output.WriteLine($"| {e,-100} |");
                            }

                            foundExecutable = locateFilesRsp.Executable.FirstOrDefault(x => x.EndsWith("Treatment.UI.Start\\bin\\x64\\Debug\\Treatment.UIStart.exe"));
                            if (string.IsNullOrWhiteSpace(foundExecutable))
                            {
                                foundExecutable = locateFilesRsp.Executable.FirstOrDefault();
                            }

                            if (string.IsNullOrWhiteSpace(foundExecutable))
                                output.WriteLine("NOTHING FOUND");
                            else
                                output.WriteLine($">>> {foundExecutable}");
                        }
                        else
                        {
                            output.WriteLine($"| {t,-100} |");
                            output.WriteLine($"| {p,-100} |");
                        }
                    }
                    else
                    {
                        foreach (var frame in rsp)
                        {
                            var s = frame.ReadString();
                            output.WriteLine($"| {s,-100} |");
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(foundExecutable))
            {
                (type, payload) = TestAgentRequestResponseSerializer.Serialize(new StartSutRequest());

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

                            output.WriteLine($"| {t,-100} |");
                            output.WriteLine($"| {p,-100} |");
                        }
                        else
                        {
                            foreach (var frame in rsp)
                            {
                                var s = frame.ReadString();
                                output.WriteLine($"| {s,-100} |");
                            }
                        }
                    }
                }
            }
        }

        [Fact]
        public void ClickMouse()
        {
            int x = 251;
            int y = 483;

            var pos = new Point { X = x, Y = y };
            var (type, payload) = InputRequestResponseSerializer.Serialize(new MoveMouseToRequest { Position = pos });

            var msg = new ZMessage(new List<ZFrame> { new ZFrame("SUT"), new ZFrame(type), new ZFrame(payload), });

            if (socket.TrySend(msg))
            {
                if (socket.TryReceive(out var rsp))
                {
                    foreach (var frame in rsp)
                    {
                        var s = frame.ReadString();
                        output.WriteLine($"| {s,-100} |");
                    }
                }
            }
        }
    }
}
