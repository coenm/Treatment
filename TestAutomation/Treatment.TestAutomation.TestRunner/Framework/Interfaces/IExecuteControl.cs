namespace Treatment.TestAutomation.TestRunner.Framework.Interfaces
{
    using System.Threading.Tasks;

    using TestAgent.Contract.Interface;

    public interface IExecuteControl
    {
        Task<IControlResponse> ExecuteControl(IControlRequest request);
    }
}
