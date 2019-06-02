namespace Treatment.TestAutomation.Contract.Interfaces.Events.Application
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class ApplicationExit : TestElementEventBase
    {
        public int ApplicationExitCode { get; set; }
    }
}
