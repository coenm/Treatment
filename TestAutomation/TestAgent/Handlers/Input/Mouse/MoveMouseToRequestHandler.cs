namespace TestAgent.Handlers.Input.Mouse
{
    using System.Threading.Tasks;
    using Implementation;
    using Interface;
    using JetBrains.Annotations;

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
