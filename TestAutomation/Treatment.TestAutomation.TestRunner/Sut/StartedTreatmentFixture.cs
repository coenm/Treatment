namespace Treatment.TestAutomation.TestRunner.Sut
{
    using System;

    using JetBrains.Annotations;
    using SimpleInjector;
    using Treatment.TestAutomation.TestRunner.Framework;
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

            Agent = new TestAgent(new MyExecuteControl(socketFactory, $"tcp://localhost:{Settings.AgentReqRspPort}"));
            // Agent.StartSutAsync();

            Mouse = new ZeroMqRemoteMouse(new MyExecuteInput(socketFactory, $"tcp://localhost:{Settings.AgentReqRspPort}"));

            Keyboard = new ZeroMqRemoteKeyboard();
        }

        public IApplication Application { get; }

        public ITestAgent Agent { get; }

        public IMouse Mouse { get; }

        public IKeyboard Keyboard { get; }

        public void Dispose()
        {
            Agent.Dispose();
            container.Dispose();
            bootstrapper.Dispose();
        }
    }
}
