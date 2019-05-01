namespace TestAgent.ZeroMq
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using TestAgent.ZeroMq.Utils;

    using ZeroMQ;

    internal class ZmqProxy
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        private readonly object syncLock = new object();
        private readonly ManualResetEvent proxyStartedSignal;
        private readonly ZSocket controlSocketSub;
        private readonly ZSocket controlSocketPub;
        private readonly ILogger logger;

        private Task runningProxyTask;
        private bool disposed;

        private ZmqProxy(ZContext context, string controlChannel, ILogger logger)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(controlChannel))
                throw new ArgumentNullException(nameof(controlChannel));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            controlSocketPub = new ZSocket(context, ZSocketType.PUB) { Linger = TimeSpan.Zero };
            controlSocketSub = new ZSocket(context, ZSocketType.SUB) { Linger = TimeSpan.Zero };
            controlSocketSub.SubscribeAll();

            if (!controlSocketSub.TryBind(controlChannel))
            {
                this.logger.Error($"Could not bind to control channel '{controlChannel}'");
                throw new ApplicationException();
            }

            if (!controlSocketPub.TryConnect(controlChannel))
            {
                this.logger.Error($"Could not connect to control channel '{controlChannel}'");
                throw new ApplicationException();
            }

            proxyStartedSignal = new ManualResetEvent(false);

            State = ProxyState.Initialized;
        }

        public ProxyState State { get; private set; }

        private static string GenerateChannelName()
        {
            return $"inproc://gen_{nameof(ZmqProxy)}_{DateTime.Now:ddhhmmssfff}_{Random.Next(10000)}";
        }

        private void StartProxy(ZSocket frontend, ZSocket backend, ZSocket capture = null)
        {
            void StartProxying()
            {
                lock (syncLock)
                {
                    if (State != ProxyState.Initialized)
                    {
                        State = ProxyState.Terminated;
                        proxyStartedSignal.Set();
                        return;
                    }

                    State = ProxyState.Running;
                    proxyStartedSignal.Set();
                }

                var proxyResult = ZContext.ProxySteerable(frontend, backend, capture, controlSocketSub, out var error);

                State = ProxyState.Terminated;

                if (proxyResult && (error == null || ZError.None.Equals(error)))
                {
                    logger.Debug("ZmqProxy closed");
                    return;
                }

                logger.Warn($"The ZmqProxy could not be closed normally. {error.Text}");
            }

            runningProxyTask = Task.Run(() => StartProxying());
        }

        /// <summary>
        /// </summary>
        /// <param name="context">current context</param>
        /// <param name="frontend">socket, already setup and connected/bound</param>
        /// <param name="backend">socket, already setup and connected/bound</param>
        /// <param name="capture"></param>
        /// <param name="logger">Logger to use</param>
        /// <exception cref="ApplicationException">Thrown when the proxy could not start.</exception>
        /// <returns></returns>
        public static ZmqProxy CreateAndRun(ZContext context, ZSocket frontend, ZSocket backend, ZSocket capture = null, ILogger logger = null)
        {
            const int startingTimeoutSec = 10;
            var result = new ZmqProxy(context, GenerateChannelName(), logger ?? new EmptyLogger());
            result.StartProxy(frontend, backend, capture);
            var signaled = result.proxyStartedSignal.WaitOne(startingTimeoutSec * 1000);
            if (signaled)
                return result;

            result.Dispose();
            throw new ApplicationException($"Could not create and start a proxy within {startingTimeoutSec} seconds.");
        }

        public void Pause()
        {
            if (disposed)
                return;

            lock (syncLock)
            {
                if (disposed)
                    return;

                if (State != ProxyState.Running)
                    return;

                using (var frame = new ZFrame("PAUSE"))
                {
                    if (!controlSocketPub.TrySend(frame))
                        return;
                }

                State = ProxyState.Paused;
            }
        }

        public void Resume()
        {
            if (disposed)
                return;

            lock (syncLock)
            {
                if (disposed)
                    return;

                if (State != ProxyState.Paused)
                    return;

                using (var frame = new ZFrame("RESUME"))
                {
                    if (!controlSocketPub.TrySend(frame))
                        return;
                }

                State = ProxyState.Running;
            }
        }

        public void Dispose()
        {
            if (disposed)
                return;

            lock (syncLock)
            {
                if (disposed)
                    return;

                using (var frame = new ZFrame("TERMINATE"))
                {
                    if (controlSocketPub.TrySend(frame, out _))
                    {
                        runningProxyTask.GetAwaiter().GetResult();
                    }
                }

                TryAndLog(() => runningProxyTask.Dispose());
                runningProxyTask = null;

                State = ProxyState.Terminated;

                TryAndLog(() => controlSocketPub.Close());
                TryAndLog(() => controlSocketPub.Dispose());

                TryAndLog(() => controlSocketSub.Close());
                TryAndLog(() => controlSocketSub.Dispose());

                TryAndLog(() => proxyStartedSignal.Dispose());

                disposed = true;
            }
        }

        private void TryAndLog(Action action, Action<string> failAction = null)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                if (failAction != null)
                    TryAndLog(() => failAction.Invoke(e.Message), null);
            }
        }
    }
}
