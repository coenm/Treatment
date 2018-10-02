namespace Treatment.Contract
{
    using System.Threading.Tasks;

    // ReSharper disable once TypeParameterCanBeVariant
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task ExecuteAsync(TCommand command);
    }
}