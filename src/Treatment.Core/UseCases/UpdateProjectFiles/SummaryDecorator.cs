﻿namespace Treatment.Core.UseCases.UpdateProjectFiles
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Treatment.Contract;
    using Treatment.Core.Interfaces;

    // todo only for UpdateProjectFilesCommand
    public class SummaryDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> decorated;
        private readonly ISummaryWriter summaryWriter;

        public SummaryDecorator(ICommandHandler<TCommand> decorated, ISummaryWriter summaryWriter)
        {
            this.decorated = decorated;
            this.summaryWriter = summaryWriter;
        }

        public async Task ExecuteAsync(TCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default(CancellationToken))
        {
            await decorated.ExecuteAsync(command, progress, ct).ConfigureAwait(false);
            summaryWriter.OutputSummary();
        }
    }
}
