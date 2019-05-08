namespace Treatment.Plugin.TestAutomation.UI.UserInput
{
    using System.Threading.Tasks;
    using global::TestAutomation.Input.Contract.Interface;

    public interface IRequestDispatcher
    {
        Task<IInputResponse> ProcessAsync(IInputRequest request);
    }
}