namespace TestAgent.Model.Configuration
{
    using JetBrains.Annotations;

    public class TestAgentApplicationSettings
    {
        [NotNull]
        public string Executable { get; set; }
    }
}
