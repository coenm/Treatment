namespace TestAgent.ZeroMq.RequestReplyInfrastructure
{
    public class ZeroMqReqRepProxyConfig
    {
        public ZeroMqReqRepProxyConfig(string frontendAddress, string backendAddress)
        {
            FrontendAddress = frontendAddress;
            BackendAddress = backendAddress;
        }

        public string FrontendAddress { get; set; }//  = Settings.Default.CommandQueryEndpoint;

        public string CaptureAddress { get; set; } // = Settings.Default.CommandQueryCaptureEndpoint;

        public string BackendAddress { get; set; } = "inproc://req-rsp";
    }
}
