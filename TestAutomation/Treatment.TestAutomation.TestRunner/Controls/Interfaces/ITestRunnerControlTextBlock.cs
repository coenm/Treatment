namespace Treatment.TestAutomation.TestRunner.Controls.Interfaces
{
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface;

    internal interface ITestRunnerControlTextBlock :
        ITextBlock,
        IPositionUpdated,
        ISizeUpdated,
        IIsEnabledChanged,
        ITextValueChanged,
        IControl,
        ITestRunnerControlPositionable
    {
    }
}