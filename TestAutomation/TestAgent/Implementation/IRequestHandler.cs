namespace TestAgent.Implementation
{
    using System.Threading.Tasks;
    using Contract.Interface;
    using JetBrains.Annotations;

    public interface IRequestHandler
    {
        Task<IResponse> ExecuteAsync(IRequest request);

        bool CanHandle(IRequest request);
    }
}
