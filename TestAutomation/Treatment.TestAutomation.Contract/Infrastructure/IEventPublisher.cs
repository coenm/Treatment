namespace Treatment.TestAutomation.Contract.Infrastructure
{
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    public interface IEventPublisher
    {
        Task PublishAsync([NotNull] TestAutomationEvent evt);
    }
}
