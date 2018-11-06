﻿namespace Treatment.UI.Implementations.Delay
{
    using JetBrains.Annotations;
    using SimpleInjector;
    using Treatment.Contract;
    using Treatment.Helpers.Guards;

    /// <summary>
    /// Adding delays is only to see if the UI is responsive etc. etc.
    /// </summary>
    public static class DelayCommandExecution
    {
        public static void Register([NotNull] Container container)
        {
            Guard.NotNull(container, nameof(container));

            container.Register<IDelayService>(() => new RandomDelayService(2000, 10000), Lifestyle.Singleton);
            container.RegisterDecorator<IDelayService, UpdateDelayStatusModelDecorator>(Lifestyle.Singleton);

            container.RegisterDecorator(
                                        typeof(ICommandHandler<>),
                                        typeof(CommandHandlerDelayDecorator<>),
                                        Lifestyle.Scoped);
        }
    }
}
