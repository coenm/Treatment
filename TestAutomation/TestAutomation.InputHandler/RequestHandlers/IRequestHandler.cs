namespace TestAutomation.InputHandler.RequestHandlers
{
    using System.Threading.Tasks;

    using TestAutomation.Input.Contract.Interface;

    public interface IRequestHandler
    {
        Task<IInputResponse> ExecuteAsync(IInputRequest request);

        bool CanHandle(IInputRequest request);
    }
}
