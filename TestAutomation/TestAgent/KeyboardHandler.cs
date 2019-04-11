namespace TestAgent
{
    using Dapplo.Windows.Input.Enums;
    using Dapplo.Windows.Input.Keyboard;
    using Interface;

    public class KeyboardHandler : IKeyboard
    {
        public IKeyboard KeyPress(char c)
        {
            KeyboardInputGenerator.KeyPresses(VirtualKeyCode.KeyA);
            return this;
        }

        public IKeyDown KeyDown(char c)
        {
            KeyboardInputGenerator.KeyDown(VirtualKeyCode.KeyB);
            return null; //todo
        }
    }
}
