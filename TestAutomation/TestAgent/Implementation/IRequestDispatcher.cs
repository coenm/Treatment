namespace TestAgent.Implementation
{
    using System.Threading.Tasks;
    using Interface;

    public interface IRequestDispatcher
    {
        Task<IResponse> ProcessAsync(IRequest request);
    }
}
