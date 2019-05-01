namespace TestAgent.ZeroMq.RequestReplyWorker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using TestAgent.Implementation;
    using TestAgent.ZeroMq.RequestReplyInfrastructure;
    using Treatment.Helpers.Guards;
    using TreatmentZeroMq.ContextService;
    using TreatmentZeroMq.Helpers;
    using ZeroMQ;

    public class ReqRepWorkerManagement : IWorker
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);
        [NotNull] private readonly IZeroMqContextService contextService;
        [NotNull] private readonly IZeroMqReqRepProxyFactory reqRspServiceFactory;

        public ReqRepWorkerManagement(
            [NotNull] IZeroMqContextService contextService,
            [NotNull] IZeroMqReqRepProxyFactory reqRspServiceFactory)
        {
            Guard.NotNull(contextService, nameof(contextService));
            Guard.NotNull(reqRspServiceFactory, nameof(reqRspServiceFactory));

            this.contextService = contextService;
            this.reqRspServiceFactory = reqRspServiceFactory;
        }

        private static string GenerateChannelName()
        {
            return $"inproc://gen_{nameof(ReqRepWorkerManagement)}_{DateTime.Now:ddhhmmssfff}_{Random.Next(10000)}";
        }

        private static IEnumerable<T> CreateEnumerable<T>(params T[] items)
        {
            return items.AsEnumerable();
        }

        public async Task StartSingleWorker(
            [NotNull] IZeroMqRequestDispatcher messageDispatcher,
            [NotNull] string backendAddress,
            CancellationToken ct = default)
        {
            Guard.NotNull(messageDispatcher, nameof(messageDispatcher));
            Guard.NotNullOrWhiteSpace(backendAddress, nameof(backendAddress));

            await Task.Yield();

//            logger.Debug("Starting zero mq worker");
            var socketName = GenerateChannelName();

            var ctx = contextService.GetContext();

            using (var cancelSocketSend = new ZSocket(ctx, ZSocketType.PUB))
            using (var cancelSocketReceive = new ZSocket(ctx, ZSocketType.SUB))
            using (var workerSocket = new ZSocket(ctx, ZSocketType.REP))
            {
                if (!cancelSocketReceive.TryBind(socketName))
                {
//                    logger.Warn("Worker could not bind to (receiving) cancel socket");
                    return;
                }
                cancelSocketReceive.SubscribeAll();

                if (!cancelSocketSend.TryConnect(socketName))
                {
//                    logger.Warn("Worker could not connect to (sending) cancel socket");
                    return;
                }

                if (!workerSocket.TryConnect(backendAddress))
                {
//                    logger.Warn("Worker could not connect to proxy socket");
                    return;
                }

                // ReSharper disable once AccessToDisposedClosure
                // The registration is disposed before the cancelSocketSend is disposed.
                using (ct.Register(() => ZmqSend.TryPoke(cancelSocketSend)))
                {
                    // To prevent deadlocks (ie, token was canceled just before the registration so no the cancelSocket wasn't 'poked')
                    if (ct.IsCancellationRequested)
                        return;

//                    logger.Info("Zero mq worker started");
                    await HandleIncomingMessagesAsync(messageDispatcher, workerSocket, cancelSocketReceive)
                        .ConfigureAwait(false);
                }

//                logger.Info("Zero mq worker stopped");
            }
        }

        private async Task HandleIncomingMessagesAsync(IZeroMqRequestDispatcher zMessageHandler, ZSocket workerSocket, ZSocket cancelSocket)
        {
            // sockets[0] is the cancel socket;
            // sockets[1] is the worker socket.
            var sockets = CreateEnumerable(cancelSocket, workerSocket).ToList();

            var polls = ZmqPolls.CreateReceiverPolls(sockets.Count);
            var continueRunning = true;

            while (continueRunning)
            {
                if (!sockets.PollIn(polls, out var messages, out var error))
                {
                    if (Equals(error, ZError.EAGAIN))
                        continue;

//                    if (Equals(error, ZError.ETERM))
//                        logger.Warn($"ZeroMq Req/Rep worker terminated. {error.Text}.");
//                    else
//                        logger.Error($"ZeroMq Req/Rep worker stopped. {error.Text}.");

                    return;
                }

                if (messages[0] != null)
                {
                    // This message is sent using the cancel socket.
                    // Therefore we don't even want to investigate the message
                    // but make sure this loop stops.
//                    logger.Info("ZeroMq Req/Rep worker received 'Stop' message -> stop working.");
                    continueRunning = false;
                    messages[0].Dispose();
                    messages[0] = null;
                }

                else if (messages[1] != null)
                {
                    // This is a real message for the worker
                    // Handle the message and send back the result.
//                    logger.Debug("ZeroMq Req/Rep worker received message -> process it.");
                    using (messages[1])
                    {
                        var result = await zMessageHandler.ProcessAsync(messages[1]).ConfigureAwait(false);

//                        logger.Debug("ZeroMq Req/Rep worker sending back response.");
                        using (result)
                        {
                            if (ZmqSend.TrySend(workerSocket, result, out error))
                                continue;

//                            if (Equals(error, ZError.ETERM))
//                                logger.Warn($"ZeroMq Req/Rep worker terminated while sending. {error.Text}.");
//                            else
//                                logger.Error($"ZeroMq Req/Rep worker experienced error while sending {error.Text}. Worker has stopped.");

                            return;
                        }
                    }
                }

                else
                {
                    // this should never happen, strange. we stop the loop.
//                    logger.Error("Strange things happen in the Zero Mq worker. Therefore it is stopped. Worker sockets.PollIn returned but it wasn't message[0] or message[1].");
                    continueRunning = false;
                }
            }
        }
    }
}
