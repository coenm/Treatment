﻿namespace CoenM.ZeroMq.Proxy
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using CoenM.ZeroMq.Helpers;
    using JetBrains.Annotations;
    using NLog;
    using Treatment.Helpers.Guards;
    using ZeroMQ;

    public class ZmqProxy : IDisposable
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly object syncLock = new object();
        private readonly ManualResetEvent proxyStartedSignal;
        private readonly ZSocket controlSocketSub;
        private readonly ZSocket controlSocketPub;

        private Task runningProxyTask;
        private bool disposed;

        private ZmqProxy([NotNull] ZContext context, [NotNull] string controlChannel)
        {
            Guard.NotNull(context, nameof(context));
            Guard.NotNullOrWhiteSpace(controlChannel, nameof(controlChannel));

            controlSocketPub = new ZSocket(context, ZSocketType.PUB) { Linger = TimeSpan.Zero };
            controlSocketSub = new ZSocket(context, ZSocketType.SUB) { Linger = TimeSpan.Zero };
            controlSocketSub.SubscribeAll();

            if (!controlSocketSub.TryBind(controlChannel))
            {
                Logger.Error($"Could not bind to control channel '{controlChannel}'");
                throw new ApplicationException();
            }

            if (!controlSocketPub.TryConnect(controlChannel))
            {
                Logger.Error($"Could not connect to control channel '{controlChannel}'");
                throw new ApplicationException();
            }

            proxyStartedSignal = new ManualResetEvent(false);

            State = ProxyState.Initialized;
        }

        [PublicAPI]
        public ProxyState State { get; private set; }

        /// <summary>Create and run the proxy.</summary>
        /// <param name="context">Current context.</param>
        /// <param name="frontend">Frontend socket, already setup and connected/bound.</param>
        /// <param name="backend">Backend socket, already setup and connected/bound.</param>
        /// <param name="capture">Socket where all the captured content is published on. Can be <c>null</c>.</param>
        /// <exception cref="ApplicationException">Thrown when the proxy could not start.</exception>
        /// <returns>The proxy.</returns>
        public static ZmqProxy CreateAndRun(
            [NotNull] ZContext context,
            [NotNull] ZSocket frontend,
            [NotNull] ZSocket backend,
            [CanBeNull] ZSocket capture = null)
        {
            Guard.NotNull(context, nameof(context));
            Guard.NotNull(frontend, nameof(frontend));
            Guard.NotNull(backend, nameof(backend));

            const int startingTimeoutSec = 10;

            var result = new ZmqProxy(context, GenerateChannelName());
            result.StartProxy(frontend, backend, capture);
            var signaled = result.proxyStartedSignal.WaitOne(startingTimeoutSec * 1000);
            if (signaled)
                return result;

            result.Dispose();
            throw new ApplicationException($"Could not create and start a proxy within {startingTimeoutSec} seconds.");
        }

        [PublicAPI]
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
                    Logger.Debug("ZmqProxy closed");
                    return;
                }

                Logger.Warn($"The ZmqProxy could not be closed normally. {error.Text}");
            }

            runningProxyTask = Task.Run(() => StartProxying());
        }

        [PublicAPI]
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

        private static string GenerateChannelName()
        {
            return $"inproc://gen_{nameof(ZmqProxy)}_{DateTime.Now:ddhhmmssfff}_{Random.Next(10000)}";
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
