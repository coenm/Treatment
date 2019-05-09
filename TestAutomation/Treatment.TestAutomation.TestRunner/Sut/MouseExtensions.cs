namespace Treatment.TestAutomation.TestRunner.Sut
{
    using System.Threading.Tasks;

    public static class MouseExtensions
    {
        public static async Task DragAsync(this IMouse mouse, int x, int y, int xDest, int yDest)
        {
            await mouse.MoveCursorAsync(x, y);
            await Task.Delay(100);

            await mouse.ClickAsync();
            await Task.Delay(100);

            await mouse.MouseDownAsync();
            await Task.Delay(100);

            await mouse.MoveCursorAsync(xDest, yDest);
            await Task.Delay(100);

            await mouse.MouseUpAsync();
        }
    }
}
