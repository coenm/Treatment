namespace Treatment.TestAutomation.TestRunner.Sut
{
    using System;

    using global::Treatment.TestAutomation.TestRunner.Framework;
    using JetBrains.Annotations;
    using SimpleInjector;
    using TreatmentZeroMq.Socket;

    [UsedImplicitly]
    public class TreatmentFixture : ISut, IDisposable
    {
        [NotNull] private readonly Bootstrapper bootstrapper;
        [NotNull] private readonly Container container;

        public TreatmentFixture()
        {
            bootstrapper = new Bootstrapper();
            container = bootstrapper.RegisterAll();

            var socketFactory = container.GetInstance<IZeroMqSocketFactory>();

            Agent = new TestAgent(new MyExecuteControl(socketFactory, $"tcp://localhost:{Settings.AgentReqRspPort}"));
            Mouse = new ZeroMqRemoteMouse(new MyExecuteInput(socketFactory, $"tcp://localhost:{Settings.AgentReqRspPort}"));
            Keyboard = new ZeroMqRemoteKeyboard(new MyExecuteInput(socketFactory, $"tcp://localhost:{Settings.AgentReqRspPort}"));
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
