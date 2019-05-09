namespace TestAgent.Implementation
{
    using System.Threading.Tasks;

    using TestAgent.Contract.Interface;

    public interface IRequestDispatcher
    {
        Task<IControlResponse> ProcessAsync(IControlRequest request);
    }
}
