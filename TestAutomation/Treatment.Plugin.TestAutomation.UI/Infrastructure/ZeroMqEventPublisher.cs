namespace Treatment.Plugin.TestAutomation.UI.Infrastructure
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using NetMQ;
    using NetMQ.Sockets;
    using NLog;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Settings;
    using Treatment.TestAutomation.Contract.Interfaces.Events;
    using Treatment.TestAutomation.Contract.Serializer;

    internal class ZeroMqEventPublisher : IEventPublisher, IDisposable
    {
        [NotNull] private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        [NotNull] private readonly object syncLock = new object();
        [NotNull] private readonly ITestAutomationSettings settings;
        [NotNull] private readonly PublisherSocket socket;
        private bool initialized;

        public ZeroMqEventPublisher([NotNull] ITestAutomationSettings settings)
        {
            Guard.NotNull(settings, nameof(settings));
            this.settings = settings;
            socket = new PublisherSocket();
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse", Justification = "Input validation despite of [NotNull] attribute.")]
        [SuppressMessage("ReSharper", "HeuristicUnreachableCode", Justification = "Input validation despite of [NotNull] attribute.")]
        public Task PublishAsync(Guid guid, IEvent evt)
        {
            if (evt == null)
                return Task.CompletedTask;

            evt.Guid = guid;
            return PublishAsync(evt);
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse", Justification = "Input validation despite of [NotNull] attribute.")]
        [SuppressMessage("ReSharper", "HeuristicUnreachableCode", Justification = "Input validation despite of [NotNull] attribute.")]
        public Task PublishAsync(IEvent evt)
        {
            if (evt == null)
                return Task.CompletedTask;

            Initialize();

            try
            {
                var (type, payload) = EventSerializer.Serialize(evt);

                socket
                    .SendMoreFrame(type)
                    .SendFrame(payload);
            }
            catch (Exception e)
            {
                socket
                    .SendMoreFrame(e.GetType().FullName)
                    .SendFrame(e.Message);
            }

            return Task.FromResult(true);
        }

        public void Dispose()
        {
            if (initialized == false)
                return;

            lock (syncLock)
            {
                if (initialized == false)
                    return;

                socket.Dispose();

                initialized = false;
            }
        }

        private void Initialize()
        {
            if (initialized)
                return;

            lock (syncLock)
            {
                if (initialized)
                    return;

                socket.Options.Linger = TimeSpan.Zero;
                socket.Options.TcpKeepalive = true;
                socket.Options.SendHighWatermark = 10_000;

                try
                {
                    socket.Connect(settings.ZeroMqEventPublishSocket);
                    initialized = true;
                }
                catch (NetMQ.EndpointNotFoundException e)
                {
                    Logger.Error(e, () => $"Could not connect. Endpoint ({settings.ZeroMqEventPublishSocket}) not found.");
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Could not connect");
                }
            }
        }
    }
}
