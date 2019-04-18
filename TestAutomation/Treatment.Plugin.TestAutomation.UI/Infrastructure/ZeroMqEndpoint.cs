namespace Treatment.Plugin.TestAutomation.UI.Infrastructure
{
    using System;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;

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
