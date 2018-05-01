namespace Treatment.Core.UseCases.UpdateProjectFiles
{
    using Treatment.Core.Interfaces;
    using Treatment.Contract;

    // todo only for UpdateProjectFilesCommand
    public class SummaryDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _decorated;
        private readonly ISummaryWriter _summaryWriter;

        public SummaryDecorator(ICommandHandler<TCommand> decorated, ISummaryWriter summaryWriter)
        {
            _decorated = decorated;
            _summaryWriter = summaryWriter;
        }

        public void Execute(TCommand command)
        {
            _decorated.Execute(command);
            _summaryWriter.OutputSummary();
        }
    }
}