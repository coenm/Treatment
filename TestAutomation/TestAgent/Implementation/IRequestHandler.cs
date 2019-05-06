namespace TestAgent.Implementation
{
    using System.Threading.Tasks;
    using Contract.Interface;

    public interface IRequestHandler
    {
        Task<IResponse> ExecuteAsync(IRequest request);

        bool CanHandle(IRequest request);
    }
}
