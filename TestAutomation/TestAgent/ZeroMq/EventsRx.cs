namespace TestAgent.ZeroMq
{
    using System;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;

    using CoenM.ZeroMq.Socket;
    using JetBrains.Annotations;
    using NLog;
    using Treatment.Helpers.Guards;
    using ZeroMQ;

    internal class EventsRx
    {
        [NotNull] private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        [NotNull] private readonly Subject<bool> subject;

        public EventsRx(
            [NotNull] IZeroMqSocketFactory socketFactory,
            [NotNull] string endpoint)
        {
            Guard.NotNull(socketFactory, nameof(socketFactory));
            Guard.NotNullOrWhiteSpace(endpoint, nameof(endpoint));

            subject = new Subject<bool>();

            // todo keep over control task.
            Task.Run(() =>
            {
                using (var subscriber = socketFactory.Create(ZSocketType.SUB))
                {
                    subscriber.Connect(endpoint);
                    subscriber.SubscribeAll();

                    while (true)
                    {
                        var zmsg = new ZMessage();

                        if (!subscriber.ReceiveMessage(ref zmsg, ZSocketFlags.None, out var error))
                        {
                            Logger.Warn($" Oops, could not receive a request: {error}");
                            subject.OnCompleted();
                            return;
                        }

                        subject.OnNext(true);
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
        public IObservable<bool> Events => subject;
    }
}
