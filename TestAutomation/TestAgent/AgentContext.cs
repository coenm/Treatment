namespace TestAgent
{
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using NLog;
    using TestAgent.Contract.Interface.Events;
    using TestAgent.ZeroMq.PublishInfrastructure;
    using Treatment.Helpers.Guards;

    public class AgentContext : IAgentContext
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly CancellationTokenSource cancellationTokenSource;
        [NotNull] private readonly ITestAgentEventPublisher publisher;
        private Medallion.Shell.Command command;

        public AgentContext([NotNull] ITestAgentEventPublisher publisher)
        {
            Guard.NotNull(publisher, nameof(publisher));
            this.publisher = publisher;

            cancellationTokenSource = new CancellationTokenSource();
            WorkingDirectory = null;
        }

        public CancellationToken CancellationToken => cancellationTokenSource.Token;

        public string WorkingDirectory { get; private set; }

        public void SetSutProcess([NotNull] Medallion.Shell.Command cmd)
        {
            Guard.NotNull(cmd, nameof(cmd));
            command = cmd;

            Task.Run(async () => await StartMonitoring(command), CancellationToken.None);
        }

        public void SetWorkingDirectory(string workingDirectory) => WorkingDirectory = workingDirectory;

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        private async Task StartMonitoring([NotNull] Medallion.Shell.Command cmd)
        {
            Logger.Info("Monitoring task..");
            var result = await cmd.Task;

            Logger.Info("Task finished..");
            Logger.Info(result.StandardOutput);
            Logger.Info(result.StandardError);
            Logger.Info($"ExitCode: {result.ExitCode}");
            Logger.Info(result.Success ? "success" : "error");

            var evt = new SutProcessStopped
                    {
                        StandardOutput = result.StandardOutput,
                        StandardError = result.StandardError,
                        ExitCode = result.ExitCode,
                        Success = result.Success,
                    };

            await publisher.PublishAsync(evt);
        }
    }
}
