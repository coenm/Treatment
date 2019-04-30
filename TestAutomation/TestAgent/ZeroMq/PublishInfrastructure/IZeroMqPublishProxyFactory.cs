namespace TestAgent.ZeroMq.PublishInfrastructure
{
    public interface IZeroMqPublishProxyFactory
    {
        ZeroMqPublishProxyConfig GetConfig();

        ZeroMqPublishProxyService Create();
    }
}
