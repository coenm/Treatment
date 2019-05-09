namespace TestAgent
{
    using System.Threading;

    using JetBrains.Annotations;
    using Medallion.Shell;
    using Treatment.Helpers.Guards;

    public class AgentContext : IAgentContext
    {
        private readonly CancellationTokenSource cancellationTokenSource;
        private Command command;

        public AgentContext()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        public CancellationToken CancellationToken => cancellationTokenSource.Token;

        public void SetSutProcess([NotNull] Command command)
        {
            Guard.NotNull(command, nameof(command));
            this.command = command;
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
