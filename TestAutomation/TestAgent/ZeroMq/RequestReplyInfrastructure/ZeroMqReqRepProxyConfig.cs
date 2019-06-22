namespace TestAgent.ZeroMq.RequestReplyInfrastructure
{
    using System.Collections.Generic;

    public class ZeroMqReqRepProxyConfig
    {
        public ZeroMqReqRepProxyConfig(string[] frontendAddress, string[] backendAddress)
        {
            FrontendAddress = frontendAddress;
            BackendAddress = backendAddress;
        }

        public string[] FrontendAddress { get; }

        public string CaptureAddress { get; set; }

        public string[] BackendAddress { get; }
    }
}
