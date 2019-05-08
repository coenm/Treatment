namespace Treatment.TestAutomation.TestRunner.Framework
{
    using System;
    using SimpleInjector;
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
            container.RegisterSingleton<IZeroMqContextService, ZeroMqContextService>();
            container.RegisterSingleton<IZeroMqSocketFactory, DefaultSocketFactory>();

            return container;
        }

        public void Dispose()
        {
            container?.Dispose();
        }
    }
}
