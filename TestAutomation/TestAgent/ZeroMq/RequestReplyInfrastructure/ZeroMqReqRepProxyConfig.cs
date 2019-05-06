namespace TestAgent.ZeroMq.RequestReplyInfrastructure
{
    using System.Collections.Generic;

    public class ZeroMqReqRepProxyConfig
    {
        public ZeroMqReqRepProxyConfig(string[] frontendAddress, Dictionary<string, string> backendAddress)
        {
            FrontendAddress = frontendAddress;
            BackendAddress = backendAddress;
        }

        public string[] FrontendAddress { get; set; }//  = Settings.Default.CommandQueryEndpoint;

        public string CaptureAddress { get; set; } // = Settings.Default.CommandQueryCaptureEndpoint;

        public Dictionary<string, string> BackendAddress { get; set; }
    }
}
