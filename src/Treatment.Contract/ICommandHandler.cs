namespace Treatment.Contract
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    // ReSharper disable once TypeParameterCanBeVariant
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task ExecuteAsync(TCommand command, [CanBeNull] IProgress<ProgressData> progress = null, CancellationToken ct = default(CancellationToken));
    }
}