namespace TestAgent
{
    using System.Threading;

    using Medallion.Shell;

    public interface IAgentContext
    {
        CancellationToken CancellationToken { get; }

        void SetSutProcess(Command command);

        void Stop();
    }
}
