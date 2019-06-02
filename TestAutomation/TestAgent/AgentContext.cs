namespace TestAgent
{
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using TreatmentZeroMq.Helpers;
    using TreatmentZeroMq.Socket;
    using ZeroMQ;

    public class AgentContext : IAgentContext
    {
        private readonly IZeroMqSocketFactory socketFactory;
        private readonly CancellationTokenSource cancellationTokenSource;
        private Medallion.Shell.Command command;

        public AgentContext([NotNull] IZeroMqSocketFactory socketFactory)
        {
            Guard.NotNull(socketFactory, nameof(socketFactory));

            this.socketFactory = socketFactory;
            cancellationTokenSource = new CancellationTokenSource();
        }

        public CancellationToken CancellationToken => cancellationTokenSource.Token;

        public void SetSutProcess([NotNull] Medallion.Shell.Command command)
        {
            Guard.NotNull(command, nameof(command));
            this.command = command;

            Task.Run(async () => await StartMonitoring(command), CancellationToken.None);
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        private async Task StartMonitoring(Medallion.Shell.Command command)
        {
            var result = await command.Task;

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
