namespace Treatment.UI.ViewModel
{
    using System.Windows.Input;

    public interface IInitializableViewModel
    {
        ICommand Initialize { get; }
    }
}
