namespace Treatment.TestAutomation.Contract.Interfaces.Events.Application
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class ApplicationStarting : TestElementEventBase
    {
        public int CountDown { get; set; }
    }
}
