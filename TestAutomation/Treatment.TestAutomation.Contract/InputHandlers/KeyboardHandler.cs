namespace Treatment.TestAutomation.Contract.InputHandlers
{
    using Dapplo.Windows.Input.Enums;
    using Dapplo.Windows.Input.Keyboard;
    using Interfaces.Framework;

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
