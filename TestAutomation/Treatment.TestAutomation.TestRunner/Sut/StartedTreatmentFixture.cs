namespace Treatment.TestAutomation.TestRunner.Sut
{
    using System;
    using Treatment.TestAutomation.TestRunner.Framework;
    using JetBrains.Annotations;
    using SimpleInjector;
    using TreatmentZeroMq.Socket;

    public class StartedTreatmentFixture : ISut, IDisposable
    {
        [NotNull] private readonly Bootstrapper bootstrapper;
        [NotNull] private readonly Container container;

        public StartedTreatmentFixture()
        {
            bootstrapper = new Bootstrapper();
            container = bootstrapper.RegisterAll();

            var socketFactory = container.GetInstance<IZeroMqSocketFactory>();

            Agent = new TestAgent(socketFactory, $"tcp://localhost:{Settings.AgentReqRspPort}");
            Agent.StartSutAsync();
        }

        public IApplication Application { get; }

        public ITestAgent Agent { get; }

        public void Dispose()
        {
            Agent.Dispose();
            container.Dispose();
            bootstrapper.Dispose();
        }
    }
}
