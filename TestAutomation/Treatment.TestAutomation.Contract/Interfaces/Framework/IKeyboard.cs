namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    public interface IKeyboard
    {
        IKeyboard KeyPress(char c);

        IKeyDown KeyDown(char c);
    }
}