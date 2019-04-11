namespace TestAgent.Interface
{
    public interface IMouse
    {
        IMouse MoveMouseTo(int x, int y);

        IMouse SingleLeftClick();

        IMouse DoubleLeftClick();

        IMouse SingleRightClick();

        IMouse DoubleRightClick();
    }
}
