namespace Treatment.TestAutomation.TestRunner.Framework
{
    using System;

    using SimpleInjector;
    using Treatment.TestAutomation.TestRunner.Sut;
    using TreatmentZeroMq.ContextService;
    using TreatmentZeroMq.Socket;

    public class Bootstrapper : IDisposable
    {
        private readonly Container container;

        public Bootstrapper()
        {
            container = new Container();
        }

        public Container RegisterAll()
        {
            var settings = new StaticAgentSettings(
                $"tcp://localhost:{Settings.AgentPublishPort}",
                $"tcp://localhost:{Settings.AgentReqRspPort}");

            container.RegisterInstance<IAgentSettings>(settings);

            container.Register<IZeroMqContextService, ZeroMqContextService>(Lifestyle.Singleton);
            container.Register<IZeroMqSocketFactory, DefaultSocketFactory>(Lifestyle.Singleton);

            container.Register<IApplicationEvents, RemoteApplicationEvents>(Lifestyle.Singleton);

            container.Register<IExecuteControl, ZeroMqExecuteControl>(Lifestyle.Transient);
            container.Register<IExecuteInput, ZeroMqExecuteInput>(Lifestyle.Transient);

            container.Register<ITreatmentApplication, RemoteTreatmentApplication>(Lifestyle.Transient);
            container.Register<ITestAgent, RemoteTestAgent>(Lifestyle.Transient);
            container.Register<IMouse, RemoteMouse>(Lifestyle.Transient);
            container.Register<IKeyboard, RemoteKeyboard>(Lifestyle.Transient);

            return container;
        }

        public void Dispose()
        {
            container?.Dispose();
        }
    }
}
