namespace Treatment.TestAutomation.TestRunner.Controls.Interfaces
{
    using System.Windows;

    using Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    internal interface ITestRunnerControlPositionable :
        IPositionUpdated,
        ISizeUpdated
    {
        bool HasFocus { get; }

        Point Position { get; }

        Size Size { get; }

        bool IsEnabled { get; }
    }
}