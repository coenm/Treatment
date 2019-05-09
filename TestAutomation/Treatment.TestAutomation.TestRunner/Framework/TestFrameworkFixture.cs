namespace Treatment.TestAutomation.TestRunner.Framework
{
    using System;

    using JetBrains.Annotations;
    using SimpleInjector;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    [UsedImplicitly]
    public class TestFrameworkFixture : ITestFramework, IDisposable
    {
        [NotNull] private readonly Bootstrapper bootstrapper;
        [NotNull] private readonly Container container;

        public TestFrameworkFixture()
        {
            bootstrapper = new Bootstrapper();
            container = bootstrapper.RegisterAll();

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
        }
    }
}
