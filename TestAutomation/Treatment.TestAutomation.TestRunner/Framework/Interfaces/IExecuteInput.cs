namespace Treatment.TestAutomation.TestRunner.Framework.Interfaces
{
    using System.Threading.Tasks;

    using global::TestAutomation.Input.Contract.Interface;

    public interface IExecuteInput
    {
        Task<IInputResponse> ExecuteInput(IInputRequest request);
    }
}
