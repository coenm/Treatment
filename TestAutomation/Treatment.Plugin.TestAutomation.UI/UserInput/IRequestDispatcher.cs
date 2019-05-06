namespace Treatment.Plugin.TestAutomation.UI.UserInput
{
    using System.Threading.Tasks;
    using global::TestAutomation.Contract.Input.Interface;

    public interface IRequestDispatcher
    {
        Task<IResponse> ProcessAsync(IRequest request);
    }
}