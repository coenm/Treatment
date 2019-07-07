namespace Treatment.TestAutomation.TestRunner.Framework.Interfaces
{
    using System;
    using System.Threading.Tasks;

    using TestAgent.Contract.Interface.Input.Enums;

    public static class KeyboardExtensions
    {
        public static Task PressCharacterAsync(this IKeyboard keyboard, char c)
        {
            if (c == ' ')
                return keyboard.PressAsync(VirtualKeyCode.Space);

            if (c >= '0' && c <= '9')
            {
                var baseKey = (ushort)VirtualKeyCode.Key0;
                var diff = (short)(c - '0');
                var keyToPress = (ushort)(baseKey + diff);
                return keyboard.PressAsync((VirtualKeyCode)keyToPress);
            }

            var cLower = c;
            var useShift = false;

            if (c >= 'A' && c <= 'Z')
            {
                cLower = (char)(c + ('a' - 'Z'));
                useShift = true;
            }

            if (cLower >= 'a' && cLower <= 'z')
            {
                var baseKey = (ushort)VirtualKeyCode.KeyA;
                var diff = (short)(c - 'a');
                var keyToPress = (ushort)(baseKey + diff);

                if (useShift)
                    return keyboard.PressAsync(VirtualKeyCode.Shift, (VirtualKeyCode)keyToPress);

                return keyboard.PressAsync((VirtualKeyCode)keyToPress);
            }

            throw new NotImplementedException($"Char {c} could not be sent");
        }
    }
}
