namespace Treatment.Console.Tests.E2ETests.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using SimpleInjector;

    using Treatment.Console.Bootstrap;

    internal class FakeBootstrapper : Bootstrapper
    {
        private readonly List<Action<Container>> _actions;
        private readonly ManualResetEventSlim _verified;

        public FakeBootstrapper()
        {
            _actions = new List<Action<Container>>();
            _verified = new ManualResetEventSlim(false);
        }

        public override IDisposable StartSession()
        {
            if (!_verified.IsSet)
            {
                PostRegister();
                _verified.Set();
            }

            return base.StartSession();
        }

        public void RegisterPostRegisterAction(Action<Container> action)
        {
            _actions.Add(action);
        }

        private void PostRegister()
        {
            _actions.ForEach(a => a.Invoke(Container));
        }
    }
}