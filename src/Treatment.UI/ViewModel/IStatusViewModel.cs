namespace Treatment.UI.ViewModel
{
    using Wpf.Framework.Commands.Nito;

    public interface IStatusViewModel
    {
        CapturingExceptionAsyncCommand Initialize { get; }

        string StatusText { get; }

        string ConfigFilename { get; }

        int DelayProcessCounter { get; }
    }
}
