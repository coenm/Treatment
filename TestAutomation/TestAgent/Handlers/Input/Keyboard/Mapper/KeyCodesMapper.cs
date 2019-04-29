namespace TestAgent.Handlers.Input.Keyboard.Mapper
{
    using Dapplo.Windows.Input.Enums;

    internal static class KeyCodesMapper
    {
        public static VirtualKeyCode[] Map(Interface.Input.Enums.VirtualKeyCode[] keyCodes)
        {
            if (keyCodes == null)
                return null;

            // todo map!
            return new VirtualKeyCode[]
            {
                VirtualKeyCode.Escape,
            };
        }
    }
}
