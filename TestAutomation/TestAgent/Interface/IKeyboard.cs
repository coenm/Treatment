namespace TestAgent.Interface
{
    public interface IKeyboard
    {
        IKeyboard KeyPress(char c);

        IKeyDown KeyDown(char c);
    }
}
