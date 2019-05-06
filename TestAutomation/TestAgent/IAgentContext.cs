namespace TestAgent
{
    using System.Threading;
    using Medallion.Shell;

    public interface IAgentContext
    {
        void SetSutProcess(Command command);

        CancellationToken CancellationToken { get; }

        void Stop();
    }
}
