namespace Treatment.TestAutomation.TestRunner.Controls.Interfaces
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    // todo strip TestRunner in interface name

    internal static class MouseExtensions
    {
        public static Task MoveMouseCursorToElementAsync([NotNull] this IMouse mouse, [NotNull] ITestRunnerControlPositionable element)
        {
            if (mouse == null)
                throw new ArgumentNullException(nameof(mouse));
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var x = (int)(element.Position.X + (element.Size.Width / 2));
            var y = (int)(element.Position.Y + (element.Size.Height / 2));

            return mouse.MoveCursorAsync(x, y);
        }
    }
}
