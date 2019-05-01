namespace TestAgent.RequestHandlers.Input.Mouse
{
    using System.Threading.Tasks;
    using Contract.Interface;
    using JetBrains.Annotations;

    using TestAgent.Implementation;

    [PublicAPI]
    public class SingleClickRequestHandler : IRequestHandler
    {
        public Task<IResponse> ExecuteAsync(IRequest request)
        {
            throw new System.NotImplementedException();
        }

        public bool CanHandle(IRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
