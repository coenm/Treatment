namespace Treatment.UI.ViewModel
{
    public interface IMainWindowViewModel
    {
        string Name { get; set; }
    }


    /// <remarks>
    /// Only to be used as a DesignInstance in xaml.
    /// </remarks>>
    public abstract class DummyMainWindowViewModel : IMainWindowViewModel
    {
        public abstract string Name { get; set; }
    }
}