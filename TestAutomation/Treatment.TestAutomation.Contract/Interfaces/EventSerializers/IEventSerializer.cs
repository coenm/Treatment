namespace Treatment.TestAutomation.Contract.Interfaces.EventSerializers
{
    using Events;
    using JetBrains.Annotations;
    using ZeroMQ;

    public interface IEventSerializer
    {
        int Priority { get; }

        bool CanSerialize(IEvent evt);

        [NotNull] ZFrame[] Serialize([NotNull] IEvent evt);

        [NotNull] IEvent Deserialize([NotNull] ZFrame[] evt);
    }
}
