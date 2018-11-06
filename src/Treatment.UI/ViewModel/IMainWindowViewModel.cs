namespace Treatment.UI.ViewModel
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Input;

    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "XAML")]
    public interface IMainWindowViewModel
    {
        IProjectCollectionViewModel ProjectCollection { get; }

        ICommand OpenSettings { get; }

        IStatusViewModel StatusViewModel { get; }
    }
}
