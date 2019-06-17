namespace TestAgent.ViewModel
{
    using System.Windows.Input;

    public interface ITestAgentMainWindowViewModel
    {
        int EventsCounter { get; }
        ICommand OpenSettingsCommand { get; }
    }
}
