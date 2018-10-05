namespace Treatment.UI.ViewModel
{
    using Nito.Mvvm;

    public interface IMainWindowViewModel
    {
        string Name { get; set; }


        AsyncCommand FixCsProjectFiles { get; }
    }


    /// <remarks>
    /// Only to be used as a DesignInstance in xaml.
    /// </remarks>>
    public abstract class DummyMainWindowViewModel : IMainWindowViewModel
    {
        public abstract string Name { get; set; }

        public abstract AsyncCommand FixCsProjectFiles { get; }
    }
}