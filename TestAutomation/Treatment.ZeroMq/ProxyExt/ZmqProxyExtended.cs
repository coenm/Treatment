namespace TreatmentZeroMq.ProxyExt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Helpers;
    using Proxy;
    using ZeroMQ;

    public class ZmqProxyExtended
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        private readonly object syncLock = new object();
        private readonly ManualResetEvent proxyStartedSignal;

        private Task runningProxyTask;
        private bool disposed;

        private ZmqProxyExtended(ZContext context, string controlChannel)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(controlChannel))
                throw new ArgumentNullException(nameof(controlChannel));

            proxyStartedSignal = new ManualResetEvent(false);

            State = ProxyState.Initialized;
        }

        public ProxyState State { get; private set; }

        private static string GenerateChannelName()
        {
            return $"inproc://gen_{nameof(ZmqProxy)}_{DateTime.Now:ddhhmmssfff}_{Random.Next(10000)}";
        }

        private void StartProxy(ZSocket frontend, ZKeySocket[] backend)
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


                var sockets = new List<ZSocket> {frontend}.Concat(backend.Select(x => x.Socket)).ToList();

                var polls = ZmqPolls.CreateReceiverPolls(sockets.Count);


                var continueRunning = true;
                while (continueRunning)
                {
                    if (!sockets.PollIn(polls, out var messages, out var error))
                    {
                        if (Equals(error, ZError.EAGAIN))
                            continue;

                        return;
                    }

                    // single frontend -> determine what backend in should be routed to.
                    if (messages[0] != null)
                    {
                        using (messages[0])
                        {
                            var s = backend.FirstOrDefault(handler => handler.ShouldUseSocket(messages[0][2]));
                            if (s != null)
                            {
                                s.Socket.TrySend(messages[0]);
                            }
                            else
                            {
                                continueRunning = false;
                            }
                        }
                    }


                    // backend -> send to frontend.
                    for (var i = 1; i < messages.Length; i++)
                    {
                        if (messages[i] == null)
                            continue;

                        using (messages[i])
                        {
                            if (!frontend.TrySend(messages[i]))
                            {
                                // do something?
                                continueRunning = false;
                            }
                        }
                    }
                }
            }

            runningProxyTask = Task.Run(() => StartProxying());
        }

        /// <summary>
        /// </summary>
        /// <param name="context">current context</param>
        /// <param name="frontend">socket, already setup and connected/bound</param>
        /// <param name="backends">socket, already setup and connected/bound</param>
        /// <exception cref="ApplicationException">Thrown when the proxy could not start.</exception>
        /// <returns></returns>
        public static ZmqProxyExtended CreateAndRun(ZContext context, ZSocket frontend, ZKeySocket[] backends)
        {
            const int startingTimeoutSec = 10;
            var result = new ZmqProxyExtended(context, GenerateChannelName());
            result.StartProxy(frontend, backends);
            var signaled = result.proxyStartedSignal.WaitOne(startingTimeoutSec * 1000);
            if (signaled)
                return result;

            result.Dispose();
            throw new ApplicationException($"Could not create and start a proxy within {startingTimeoutSec} seconds.");
        }

        public void Dispose()
        {
            if (disposed)
                return;

            lock (syncLock)
            {
                if (disposed)
                    return;

                TryAndLog(() => runningProxyTask.Dispose());
                runningProxyTask = null;

                State = ProxyState.Terminated;

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
