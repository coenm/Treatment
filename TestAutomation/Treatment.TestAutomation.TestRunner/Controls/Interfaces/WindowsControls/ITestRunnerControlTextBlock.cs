namespace Treatment.TestAutomation.TestRunner.Controls.Interfaces.WindowsControls
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
        IElementLoadedUnLoaded,
        ITestRunnerControlPositionable
    {
    }
}
