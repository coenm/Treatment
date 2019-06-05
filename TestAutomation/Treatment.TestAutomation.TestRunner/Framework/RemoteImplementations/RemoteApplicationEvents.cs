namespace Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations
{
    using System;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events;
    using Treatment.TestAutomation.Contract.Serializer;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;
    using TreatmentZeroMq.Socket;
    using ZeroMQ;

    internal class RemoteApplicationEvents : IApplicationEvents
    {
        [NotNull] private readonly Subject<IEvent> subject;

        public RemoteApplicationEvents(
            [NotNull]IZeroMqSocketFactory socketFactory,
            [NotNull] IAgentSettings agentSettings)
        {
            Guard.NotNull(socketFactory, nameof(socketFactory));
            Guard.NotNull(agentSettings, nameof(agentSettings));

            subject = new Subject<IEvent>();

            var eventEndpoint = agentSettings.EventsEndpoint;

            Task.Run(() =>
            {
                using (var subscriber = socketFactory.Create(ZSocketType.SUB))
                {
                    subscriber.Connect(eventEndpoint);
                    subscriber.SubscribeAll();

                    while (true)
                    {
                        var zmsg = new ZMessage();

                        if (!subscriber.ReceiveMessage(ref zmsg, ZSocketFlags.None, out var error))
                        {
                            Console.WriteLine($" Oops, could not receive a request: {error}");
                            subject.OnCompleted();
                            return;
                        }

                        try
                        {
                            using (zmsg)
                            {
                                var @type = zmsg.Pop().ReadString();
                                var payload = zmsg.Pop().ReadString();
                                var value = EventSerializer.DeserializeEvent(@type, payload);

                                if (value != null)
                                    subject.OnNext(value);

                                //todo handle null?
                            }
                        }
                        catch (Exception e)
                        {
                            subject.OnError(e);
                            return;
                        }
                    }
                }
            }).ConfigureAwait(false);

            // Events = Observable.Create<IEvent>(
            //  observer =>
            //  {
            //      var d = new CompositeDisposable();
            //      observer.OnNext(new ApplicationActivated());
            //      return d;
            //  });
        }

        [NotNull]
        public IObservable<IEvent> Events => subject;
    }
}
