namespace Treatment.TestAutomation.TestRunner.XUnit
{
    using Treatment.TestAutomation.TestRunner.Framework;
    using Xunit;

    [CollectionDefinition(nameof(TestFramework))]
    public class TestFramework : ICollectionFixture<TestFrameworkFixture>
    {
        // This class has no code, and is never created.
        // Its purpose is simply to be the place to apply [CollectionDefinition]
        // and all the ICollectionFixture<> interfaces.
    }
}
