namespace TestAgent.Contract.Interface.Events
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class SutProcessStopped : TestAgentEventBase
    {
        public bool Success { get; set; }

        public string StandardOutput { get; set; }

        public string StandardError { get; set; }

        public int ExitCode { get; set; }
    }
}
