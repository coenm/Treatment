namespace Treatment.TestAutomation.TestRunner.Framework.Interfaces
{
    using System.Threading.Tasks;

    using TestAgent.Contract.Interface;

    public interface IExecuteInput
    {
        Task<IControlResponse> ExecuteInput(IControlRequest request);
    }
}
