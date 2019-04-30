namespace TestAgent.RequestHandlers.Input.Mouse
{
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using TestAgent.Implementation;
    using TestAgent.Interface;

    [PublicAPI]
    public class MoveMouseToRequestHandler : IRequestHandler
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
