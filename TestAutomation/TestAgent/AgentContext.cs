namespace TestAgent
{
    using System.Threading;
    using JetBrains.Annotations;
    using Medallion.Shell;
    using Treatment.Helpers.Guards;

    public class AgentContext : IAgentContext
    {
        private Command command;
        private readonly CancellationTokenSource cancellationTokenSource;

        public AgentContext()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        public void SetSutProcess([NotNull] Command command)
        {
            Guard.NotNull(command, nameof(command));
            this.command = command;
        }

        public CancellationToken CancellationToken => cancellationTokenSource.Token;
        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
