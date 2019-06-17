namespace Treatment.UI.Core.ViewModel
{
    public interface IProgressViewModel
    {
        int Value { get; }

        int Max { get; }

        bool IsIndeterminate { get; }
    }
}
