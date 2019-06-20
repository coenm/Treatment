namespace TestAgent.RequestHandlers.Input.Keyboard.Mapper
{
    using Dapplo.Windows.Input.Enums;

    internal static class KeyCodesMapper
    {
        public static VirtualKeyCode[] Map(TestAgent.Contract.Interface.Input.Enums.VirtualKeyCode[] keyCodes)
        {
            if (keyCodes == null)
                return null;

            var result = new VirtualKeyCode[keyCodes.Length];
            for (var i = 0; i < keyCodes.Length; i++)
            {
                result[i] = (VirtualKeyCode)keyCodes[i];
            }

            return result;
        }
    }
}
