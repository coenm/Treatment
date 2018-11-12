namespace Treatment.UI.ViewModel
{
    public interface IProgressViewModel
    {
        int Value { get; }

        int Max { get; }

        bool IsIndeterminate { get; }
    }
}
