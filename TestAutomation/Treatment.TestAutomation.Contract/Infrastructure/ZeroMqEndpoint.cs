namespace Treatment.TestAutomation.Contract.Infrastructure
{
    using System;
    using Helpers.Guards;
    using JetBrains.Annotations;

    public class ZeroMqEndpoint : ITestAutomationEndpoint, IDisposable
    {
        [NotNull] private readonly IZeroMqContextService contextService;

        public ZeroMqEndpoint([NotNull] IZeroMqContextService contextService)
        {
            Guard.NotNull(contextService, nameof(contextService));
            this.contextService = contextService;
        }

        public void StartAccepting()
        {
        }

        public void Dispose()
        {

        }
    }
}
