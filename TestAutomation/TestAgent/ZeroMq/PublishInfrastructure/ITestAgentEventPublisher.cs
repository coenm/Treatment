namespace TestAgent.ZeroMq.PublishInfrastructure
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using TestAgent.Contract.Interface.Events;

    public interface ITestAgentEventPublisher
    {
        Task PublishAsync([NotNull] ITestAgentEvent evt);

        Task PublishAsync(Guid guid, [NotNull] ITestAgentEvent evt);
    }
}
