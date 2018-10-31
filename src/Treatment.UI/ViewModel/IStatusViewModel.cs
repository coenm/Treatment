namespace Treatment.UI.ViewModel
{
    using Nito.Mvvm;

    public interface IStatusViewModel
    {
        CapturingExceptionAsyncCommand Initialize { get; }

        string StatusText { get; }
    }
}
