namespace Treatment.TestAutomation.TestRunner.Sut
{
    using JetBrains.Annotations;

    internal interface ITestFramework
    {
        [NotNull] ITreatmentApplication Application { get; }

        [NotNull] ITestAgent Agent { get; }

        [NotNull] IMouse Mouse { get; }

        [NotNull] IKeyboard Keyboard { get; }
    }
}
