namespace TestAgent.Implementation
{
    using System.Threading.Tasks;

    using TestAgent.Contract.Interface;

    public interface IRequestHandler
    {
        Task<IControlResponse> ExecuteAsync(IControlRequest request);

        bool CanHandle(IControlRequest request);
    }
}
