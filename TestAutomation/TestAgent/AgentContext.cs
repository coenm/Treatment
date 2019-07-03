namespace TestAgent
{
    using System.Threading;
    using System.Threading.Tasks;

    using CoenM.ZeroMq.Helpers;
    using CoenM.ZeroMq.Socket;
    using JetBrains.Annotations;
    using NLog;
    using Treatment.Helpers.Guards;
    using ZeroMQ;

    public class AgentContext : IAgentContext
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IZeroMqSocketFactory socketFactory;
        private readonly CancellationTokenSource cancellationTokenSource;
        private Medallion.Shell.Command command;

        public AgentContext([NotNull] IZeroMqSocketFactory socketFactory)
        {
            Guard.NotNull(socketFactory, nameof(socketFactory));

            this.socketFactory = socketFactory;
            cancellationTokenSource = new CancellationTokenSource();
            WorkingDirectory = null;
        }

        public CancellationToken CancellationToken => cancellationTokenSource.Token;

        public string WorkingDirectory { get; private set; }

        public void SetSutProcess([NotNull] Medallion.Shell.Command command)
        {
            Guard.NotNull(command, nameof(command));
            this.command = command;

            Task.Run(async () => await StartMonitoring(command), CancellationToken.None);
        }

        public void SetWorkingDirectory(string workingDirectory) => WorkingDirectory = workingDirectory;

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        private async Task StartMonitoring(Medallion.Shell.Command command)
        {
            Logger.Info("Monitoring task..");
            var result = await command.Task;

            Logger.Info("Task finished..");
            Logger.Info(result.StandardOutput);
            Logger.Info(result.StandardError);
            Logger.Info($"ExitCode: {result.ExitCode}");
            Logger.Info(result.Success ? "success" : "error");

            using (var socket = socketFactory.Create(ZSocketType.PUB))
            {
                if (socket.TryConnect("inproc://publish"))
                {
                    socket.Send(new ZMessage(new[]
                                             {
                                                 new ZFrame("AGENT"),
                                                 new ZFrame("SUT PROCESS"),
                                                 new ZFrame(result.StandardOutput),
                                                 new ZFrame(result.StandardError),
                                                 new ZFrame(result.ExitCode),
                                                 new ZFrame(result.Success ? "1" : "0"),
                                             }));
                }
            }
        }
    }
}
