namespace Treatment.TestAutomation.Contract.InputHandlers
{
    using Dapplo.Windows.Common.Structs;
    using Dapplo.Windows.Input.Enums;
    using Dapplo.Windows.Input.Mouse;
    using Interfaces.Framework;


    public class MouseHandler : IMouse
    {
        public IMouse MoveMouseTo(int x, int y)
        {
            MouseInputGenerator.MoveMouse(new NativePoint(x, y));
            return this;
        }

        public IMouse SingleLeftClick()
        {
            MouseInputGenerator.MouseClick(MouseButtons.Left);
            return this;
        }

        public IMouse DoubleLeftClick()
        {
            return SingleLeftClick().SingleLeftClick();
        }

        public IMouse SingleRightClick()
        {
            MouseInputGenerator.MouseClick(MouseButtons.Right);
            return this;
        }

        public IMouse DoubleRightClick()
        {
            return SingleRightClick().SingleRightClick();
        }
    }
}
