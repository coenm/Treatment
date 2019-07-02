namespace Treatment.TestAutomation.TestRunner.Controls.Interfaces
{
    using System.Windows;

    using Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    internal interface ITestRunnerControlPositionable :
        IPositionUpdated,
        ISizeUpdated
    {
        Point Position { get; }

        Size Size { get; }
    }
}
