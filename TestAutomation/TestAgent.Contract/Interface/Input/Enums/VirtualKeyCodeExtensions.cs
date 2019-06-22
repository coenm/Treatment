namespace TestAgent.Contract.Interface.Input.Enums
{
    /// <summary>
    /// Extensions for VirtualKeyCode.
    /// </summary>
    public static class VirtualKeyCodeExtensions
    {
        /// <summary>
        /// Test if the VirtualKeyCode is a modifier key.
        /// </summary>
        /// <param name="virtualKeyCode">VirtualKeyCode.</param>
        /// <returns>bool.</returns>
        public static bool IsModifier(this VirtualKeyCode virtualKeyCode)
        {
            var isModifier = false;
            switch (virtualKeyCode)
            {
            case VirtualKeyCode.Capital:
            case VirtualKeyCode.NumLock:
            case VirtualKeyCode.Scroll:
            case VirtualKeyCode.LeftShift:
            case VirtualKeyCode.Shift:
            case VirtualKeyCode.RightShift:
            case VirtualKeyCode.Control:
            case VirtualKeyCode.LeftControl:
            case VirtualKeyCode.RightControl:
            case VirtualKeyCode.Menu:
            case VirtualKeyCode.LeftMenu:
            case VirtualKeyCode.RightMenu:
            case VirtualKeyCode.LeftWin:
            case VirtualKeyCode.RightWin:
                isModifier = true;
                break;
            }

            return isModifier;
        }
    }
}
