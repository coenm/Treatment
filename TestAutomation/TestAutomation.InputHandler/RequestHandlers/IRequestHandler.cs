namespace TestAutomation.InputHandler.RequestHandlers
{
    using System.Threading.Tasks;
    using Contract.Input.Interface;

    public interface IRequestHandler
    {
        Task<IResponse> ExecuteAsync(IRequest request);

        bool CanHandle(IRequest request);
    }
}
