namespace Treatment.TestAutomation.TestRunner.Sut
{
    using System.Threading.Tasks;
    using global::TestAgent.Contract.Interface;

    public interface IExecuteControl
    {
        Task<IControlResponse> ExecuteControl(IControlRequest request);
    }
}