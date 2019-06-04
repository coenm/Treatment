namespace TestAgent.ZeroMq.PublishInfrastructure
{
    using System.Linq;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;

    public class ZeroMqPublishProxyConfig
    {
        public ZeroMqPublishProxyConfig(
            [NotNull] [ItemNotNull] string[] frontendAddress,
            [NotNull] [ItemNotNull] string[] backendAddress,
            [CanBeNull] string captureAddress = null)
        {
            Guard.NotNull(frontendAddress, nameof(frontendAddress));
            Guard.NotNull(backendAddress, nameof(backendAddress));
            Guard.NotNullOrEmpty(frontendAddress.Where(x => x != null).ToArray(), nameof(frontendAddress));
            Guard.NotNullOrEmpty(backendAddress.Where(x => x != null).ToArray(), nameof(backendAddress));

            FrontendAddress = frontendAddress.Where(x => x != null).ToArray();
            BackendAddress = backendAddress.Where(x => x != null).ToArray();
            CaptureAddress = captureAddress;
        }

        [NotNull]
        [ItemNotNull]
        public string[] FrontendAddress { get; }

        [CanBeNull]
        public string CaptureAddress { get; }

        [NotNull]
        [ItemNotNull]
        public string[] BackendAddress { get; }
    }
}
