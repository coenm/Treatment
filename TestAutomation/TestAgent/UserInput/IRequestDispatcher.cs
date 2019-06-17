namespace TestAgent.UserInput
{
    using System.Threading.Tasks;

    using TestAutomation.Input.Contract.Interface;

    public interface IRequestDispatcher
    {
        Task<IInputResponse> ProcessAsync(IInputRequest request);
    }
}
