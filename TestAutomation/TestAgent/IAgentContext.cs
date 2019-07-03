namespace TestAgent
{
    using System.IO;
    using System.Threading;
    using JetBrains.Annotations;
    using Medallion.Shell;

    public interface IAgentContext
    {
        CancellationToken CancellationToken { get; }

        [CanBeNull] string WorkingDirectory { get; }

        void SetSutProcess(Command command);

        void SetWorkingDirectory(string workingDirectory);

        void Stop();
    }
}
