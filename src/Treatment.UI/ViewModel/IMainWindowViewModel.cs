namespace Treatment.UI.ViewModel
{
    using Nito.Mvvm;

    public interface IMainWindowViewModel
    {
        string WorkingDirectory { get; set; }

        CapturingExceptionAsyncCommand FixCsProjectFiles { get; }
    }


    /// <remarks>
    /// Only to be used as a DesignInstance in xaml.
    /// </remarks>>
    public abstract class DummyMainWindowViewModel : IMainWindowViewModel
    {
        public abstract string WorkingDirectory { get; set; }

        public abstract CapturingExceptionAsyncCommand FixCsProjectFiles { get; }
    }}