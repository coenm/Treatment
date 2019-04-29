namespace Treatment.Plugin.TestAutomation.UI.Infrastructure
{
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.TestAutomation.Contract.Interfaces.Events;

    public interface IEventPublisher
    {
        Task PublishAsync([NotNull] IEvent evt);
    }
}
