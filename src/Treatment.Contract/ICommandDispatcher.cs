namespace Treatment.Contract
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    public interface ICommandDispatcher
    {
        Task ExecuteAsync<TCommand>([NotNull] TCommand command, [CanBeNull] IProgress<ProgressData> progress = null, CancellationToken ct = default)
            where TCommand : class, ICommand;
    }
}
