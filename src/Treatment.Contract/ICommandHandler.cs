namespace Treatment.Contract
{
    // ReSharper disable once TypeParameterCanBeVariant
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        void Execute(TCommand command);
    }
}