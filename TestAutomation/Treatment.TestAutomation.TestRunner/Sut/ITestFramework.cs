namespace Treatment.TestAutomation.TestRunner.Sut
{
    using JetBrains.Annotations;

    internal interface ITestFramework
    {
        [NotNull] IApplication Application { get; }

        [NotNull] ITestAgent Agent { get; }

        [NotNull] IMouse Mouse { get; }

        [NotNull] IKeyboard Keyboard { get; }
    }
}
