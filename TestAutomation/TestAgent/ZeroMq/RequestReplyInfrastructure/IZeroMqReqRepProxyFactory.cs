namespace TestAgent.ZeroMq.RequestReplyInfrastructure
{
    public interface IZeroMqReqRepProxyFactory
    {
        ZeroMqReqRepProxyConfig GetConfig();

        ZeroMqReqRepProxyService Create();
    }
}
