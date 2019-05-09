namespace Treatment.TestAutomation.TestRunner.Sut
{
    using System;

    using global::Treatment.TestAutomation.TestRunner.Framework;
    using JetBrains.Annotations;
    using SimpleInjector;
    using TreatmentZeroMq.Socket;

    [UsedImplicitly]
    public class TestFrameworkFixture : ITestFramework, IDisposable
    {
        [NotNull] private readonly Bootstrapper bootstrapper;
        [NotNull] private readonly Container container;

        public TestFrameworkFixture()
        {
            bootstrapper = new Bootstrapper();
            container = bootstrapper.RegisterAll();

            var socketFactory = container.GetInstance<IZeroMqSocketFactory>();
            var agentEndPoint = $"tcp://localhost:{Settings.AgentReqRspPort}";

            Application = new RemoteApplication();
            Agent = new RemoteTestAgent(new MyExecuteControl(socketFactory, agentEndPoint));
            Mouse = new RemoteMouse(new MyExecuteInput(socketFactory, agentEndPoint));
            Keyboard = new RemoteKeyboard(new MyExecuteInput(socketFactory, agentEndPoint));
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

