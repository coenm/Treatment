namespace TestAgent.Implementation
{
    using System.Threading.Tasks;
    using Contract.Interface;

    public interface IRequestDispatcher
    {
        Task<IResponse> ProcessAsync(IRequest request);
    }
}
