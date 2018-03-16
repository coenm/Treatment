namespace Treatment.Core.UseCases
{
    // ReSharper disable once TypeParameterCanBeVariant
    public interface ICommandHandler<TCommand> where TCommand : ITreatmentCommand
    {
        void Execute(TCommand command);
    }
}