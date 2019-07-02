namespace Treatment.TestAutomation.TestRunner.Controls.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    // todo strip TestRunner in interface name
    internal interface ITestRunnerControlButton :
        IButton,
        IButtonClicked,
        IPositionUpdated,
        IIsEnabledChanged,
        ISizeUpdated,
        IFocusChange,
        IKeyboardFocusChanged,
        IControl,
        ITestRunnerControlPositionable
    {
    }

    internal interface ITestRunnerControlComboBox :
        IComboBox,
        IPositionUpdated,
        IIsEnabledChanged,
        ISizeUpdated,
        IFocusChange,
        IKeyboardFocusChanged,
        IDropDownOpened,
        IDropDownClosed,
        ISelectionChanged,
        IControl,
        ITestRunnerControlPositionable
    {
        string SelectedItem { get; }
    }

    internal interface ITestRunnerControlCheckBox :
        ICheckBox,
        IPositionUpdated,
        IIsEnabledChanged,
        ISizeUpdated,
        IFocusChange,
        IKeyboardFocusChanged,
        ISelectionChanged,
        ICheckableChanged,
        IControl,
        ITestRunnerControlPositionable
    {
        bool IsChecked { get; }
    }

    internal interface ITestRunnerControlPositionable :
        IPositionUpdated,
        IIsEnabledChanged,
        ISizeUpdated,
        IFocusChange
    {
        bool HasFocus { get; }

        Point Position { get; }

        Size Size { get; }

        bool IsEnabled { get; }
    }

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
