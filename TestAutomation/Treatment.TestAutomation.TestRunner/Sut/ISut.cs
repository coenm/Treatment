namespace Treatment.TestAutomation.TestRunner.Sut
{
    public interface ISut
    {
        IApplication Application { get; }

        ITestAgent Agent { get; }
    }
}