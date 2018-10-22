namespace Treatment.Console.Tests.E2ETests.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using SimpleInjector;

    using Treatment.Console.Bootstrap;

    internal class FakeBootstrapper : Bootstrapper
    {
        private readonly List<Action<Container>> actions;
        private readonly ManualResetEventSlim verified;

        public FakeBootstrapper()
        {
            actions = new List<Action<Container>>();
            verified = new ManualResetEventSlim(false);
        }

        public override IDisposable StartSession()
        {
            if (!verified.IsSet)
            {
                PostRegister();
                verified.Set();
            }

            return base.StartSession();
        }

        public void RegisterPostRegisterAction(Action<Container> action)
        {
            actions.Add(action);
        }

        private void PostRegister()
        {
            actions.ForEach(a => a.Invoke(Container));
        }
    }
}
