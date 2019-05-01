namespace TestAgent.ZeroMq.RequestReplyInfrastructure
{
    using System;

    using ZeroMQ;

    /// <inheritdoc />
    ///  <summary>
    ///  ProxyService with the frontend as ROUTER and backend as DEALER. Both sockets will bind to the configurable endpoint.
    ///  Usage: One or multiple 'worker' threads can connect to the backends DEALER socket using a RSP socket and
    ///  one or more clients can connect to the frontends ROUTER port using a REQ socket.
    ///  When a client sends a request, this request is routed to an idle worker that handles the message. When the worker sends a reply,
    ///  it will be proxied back to the caller client and the worker is ready to handle a new request.
    ///  When two workers are connected to the proxy and three clients send a request at the same time, only two are processed and the third
    ///  is queued until one of the workers becomes idle.
    ///  </summary>
    public class ZeroMqReqRepProxyService : IDisposable
    {
        private ZContext ctx;
        private readonly ZeroMqReqRepProxyConfig config;
        private readonly ILogger logger;
        private ZSocket frontend;
        private ZSocket backend;
        private readonly object syncLock = new object();
        private bool socketBound;
        private ZmqProxy proxy;
        private ZSocket capture;

        public ZeroMqReqRepProxyService(ZContext context, ZeroMqReqRepProxyConfig config, ILogger logger)
        {
             ctx = context;
            this.config = config;
            this.logger = logger;

            frontend = new ZSocket(ctx, ZSocketType.ROUTER) { Linger = TimeSpan.Zero };
            backend = new ZSocket(ctx, ZSocketType.DEALER) { Linger = TimeSpan.Zero };

            if (!string.IsNullOrWhiteSpace(this.config.CaptureAddress))
            {
                this.logger.Info("Creating a capture socket for the ReqRep proxy.");
                capture = new ZSocket(ctx, ZSocketType.PUB) { Linger = TimeSpan.Zero };
            }
        }

        public void Start()
        {
            if (proxy != null)
                return;

            lock (syncLock)
            {
                if (proxy != null)
                    return;

                if (!Bind())
                    return;

                proxy = ZmqProxy.CreateAndRun(ctx, frontend, backend, capture);
            }
        }

        private bool Bind()
        {
            if (socketBound)
                return true;

            ZError error;

            foreach (var address in config.FrontendAddress)
            {
                if (!frontend.Bind(address, out error))
                {
                    logger.Error($"Frontend socket of ReqRep proxy could not bind to {address}. {error.Text}");
                    return false;
                }
            }

            foreach (var address in config.BackendAddress)
            {
                if (!backend.Bind(address, out error))
                {
                    logger.Error($"Backend socket of ReqRep proxy could not bind to {address}. {error.Text}");
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(config.CaptureAddress))
            {
                if (!capture.Bind(config.CaptureAddress, out error))
                {
                    logger.Error($"Capture socket of ReqRep proxy could not bind to {config.CaptureAddress}. {error.Text}");
                    return false;
                }
            }

            socketBound = true;
            return true;
        }

        public void Dispose()
        {
            lock (syncLock)
            {
                // Do NOT dispose the ctx. We don't own the context, only use it.
                ctx = null;

                proxy?.Dispose();
                proxy = null;

                capture?.Close();
                capture?.Dispose();
                capture = null;

                frontend?.Close();
                frontend?.Dispose();
                frontend = null;

                backend?.Close();
                backend?.Dispose();
                backend = null;

                socketBound = false;
            }
        }
    }
}
