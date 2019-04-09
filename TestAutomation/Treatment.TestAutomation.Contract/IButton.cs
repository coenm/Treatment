namespace Treatment.TestAutomation.Contract
{
    public interface IButton
    {
        bool IsEnabled { get; }
        double Width { get; }
        double Height { get; }
    }
}
