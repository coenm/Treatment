namespace TestAgent.ZeroMq.PublishInfrastructure
{
    public class ZeroMqPublishProxyConfig
    {
        public ZeroMqPublishProxyConfig(string frontendAddress, string backendAddress)
        {
            FrontendAddress = frontendAddress;
            BackendAddress = backendAddress;
        }

        public string FrontendAddress { get; set; } // = Settings.Default.PublishEventsEndpoint;

        public string CaptureAddress { get; set; } // = Settings.Default.PublishCaptureEndpoint;

        public string BackendAddress { get; set; } = "inproc://publish";
    }
}
