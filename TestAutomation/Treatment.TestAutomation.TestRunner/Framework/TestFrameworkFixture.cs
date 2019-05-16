namespace Treatment.TestAutomation.TestRunner.Framework
{
    using System;

    using JetBrains.Annotations;
    using SimpleInjector;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;
    using Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations;

    [UsedImplicitly]
    public class TestFrameworkFixture : ITestFramework, IDisposable
    {
        [NotNull] private readonly Bootstrapper bootstrapper;
        [NotNull] private readonly Container container;
        [NotNull] public readonly RemoteObjectManager store;

        public TestFrameworkFixture()
        {
            bootstrapper = new Bootstrapper();
            container = bootstrapper.RegisterAll();

            store = container.GetInstance<RemoteObjectManager>();
            Application = container.GetInstance<ITreatmentApplication>();
            Agent = container.GetInstance<ITestAgent>();
            Mouse = container.GetInstance<IMouse>();
            Keyboard = container.GetInstance<IKeyboard>();
        }

        public ITreatmentApplication Application { get; }

        public ITestAgent Agent { get; }

        public IMouse Mouse { get; }

        public IKeyboard Keyboard { get; }

        public void Dispose()
        {
            Agent.Dispose();
            container.Dispose();
            bootstrapper.Dispose();
            store.Dispose();
        }
    }
}
