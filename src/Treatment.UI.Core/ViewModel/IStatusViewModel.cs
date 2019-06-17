namespace Treatment.UI.Core.ViewModel
{
    using Nito.Mvvm;

    public interface IStatusViewModel
    {
        CapturingExceptionAsyncCommand Initialize { get; }

        string StatusText { get; }

        string ConfigFilename { get; }

        int DelayProcessCounter { get; }
    }
}
