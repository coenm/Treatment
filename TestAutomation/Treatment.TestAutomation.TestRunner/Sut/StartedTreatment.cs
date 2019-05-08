namespace Treatment.TestAutomation.TestRunner.Sut
{
    using Xunit;

    [CollectionDefinition(nameof(StartedTreatment))]
    public class StartedTreatment : ICollectionFixture<StartedTreatmentFixture>
    {
        // This class has no code, and is never created.
        // Its purpose is simply to be the place to apply [CollectionDefinition]
        // and all the ICollectionFixture<> interfaces.
    }
}