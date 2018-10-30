namespace Treatment.UI.Framework
{
    using System.Windows.Input;

    public interface IInitializableViewModel
    {
        ICommand Initialize { get; }
    }
}
