namespace TestAgent
{
    internal static class FixedSettings
    {
        /// <summary>
        /// TestAgent should publish events to this socket.
        /// </summary>
        public const string InternalPublishSocket = "inproc://publish";

        /// <summary>
        /// All messages handled by the proxy (handling events from Sut and TestAgent) are also published to this capturing socket.
        /// </summary>
        public const string InternalPublishProxyCapturingSocket = "inproc://capturePubSub";

        /// <summary>
        /// TestAgent worker connects to this socket for handling Request from the testrunner.
        /// </summary>
        public const string InternalRequestResponseWorkerSocket = "inproc://reqrsp";

        public const string AgentReqRspPort = "5555";

        public const string AgentPublishPort = "5556";

        public const string SutPublishPort = "5557";
    }
}
