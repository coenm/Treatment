namespace TestAgent.ZeroMq.PublishInfrastructure
{
    using System;

    using CoenM.ZeroMq.Proxy;
    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using ZeroMQ;

    /// <summary>
    /// ProxyService with the frontend as XPUB and backend as XSUB. Both sockets will bind to the configurable endpoint.
    /// Usage: Different threads can open a PUB socket and connect to this backends XSUB socket and publish a message simultaneously.
    /// The messages will be proxied to the frontends XPUB socket.
    /// </summary>
    public class ZeroMqPublishProxyService : IDisposable
    {
        private readonly object syncLock = new object();
        private readonly ZeroMqPublishProxyConfig config;

        private ZContext ctx;
        private ZSocket frontend;
        private ZSocket backend;
        private ZSocket capture;
        private bool socketBound;

        private ZmqProxy proxy;

        public ZeroMqPublishProxyService([NotNull] ZContext context, [NotNull]ZeroMqPublishProxyConfig config)
        {
            Guard.NotNull(context, nameof(context));
            Guard.NotNull(config, nameof(config));

            this.config = config;
            ctx = context;
            frontend = new ZSocket(ctx, ZSocketType.XPUB) { Linger = TimeSpan.Zero };
            backend = new ZSocket(ctx, ZSocketType.XSUB) { Linger = TimeSpan.Zero };

            if (string.IsNullOrWhiteSpace(this.config.CaptureAddress))
                return;

            // this.logger.Info("Creating a capture socket for the publish proxy.");
            capture = new ZSocket(ctx, ZSocketType.PUB) { Linger = TimeSpan.Zero };
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

        private bool Bind()
        {
            if (socketBound)
                return true;

            ZError error;

            foreach (var address in config.FrontendAddress)
            {
                if (!frontend.Bind(address, out error))
                {
                    // logger.Error($"Frontend socket of publish proxy could not bind to {address}. {error.Text}");
                    return false;
                }
            }

            foreach (var address in config.BackendAddress)
            {
                if (!backend.Bind(address, out error))
                {
                    // logger.Error($"Backend socket of publish proxy could not bind to {address}. {error.Text}");
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(config.CaptureAddress))
            {
                if (!capture.Bind(config.CaptureAddress, out error))
                {
                    // logger.Error($"Capture socket of publish proxy could not bind to {config.CaptureAddress}. {error.Text}");
                    return false;
                }
            }

            socketBound = true;
            return true;
        }
    }
}
