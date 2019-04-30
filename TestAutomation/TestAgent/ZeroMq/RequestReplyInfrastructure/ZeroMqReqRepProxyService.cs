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
        private ZContext _ctx;
        private readonly ZeroMqReqRepProxyConfig _config;
        private readonly ILogger _logger;
        private ZSocket _frontend;
        private ZSocket _backend;
        private readonly object _syncLock = new object();
        private bool _socketBound;
        private ZmqProxy _proxy;
        private ZSocket _capture;

        public ZeroMqReqRepProxyService(ZContext context, ZeroMqReqRepProxyConfig config, ILogger logger)
        {
             _ctx = context;
            _config = config;
            _logger = logger;

            _frontend = new ZSocket(_ctx, ZSocketType.ROUTER) { Linger = TimeSpan.Zero };
            _backend = new ZSocket(_ctx, ZSocketType.DEALER) { Linger = TimeSpan.Zero };

            if (!string.IsNullOrWhiteSpace(_config.CaptureAddress))
            {
                _logger.Info("Creating a capture socket for the ReqRep proxy.");
                _capture = new ZSocket(_ctx, ZSocketType.PUB) { Linger = TimeSpan.Zero };
            }
        }

        public void Start()
        {
            if (_proxy != null)
                return;

            lock (_syncLock)
            {
                if (_proxy != null)
                    return;

                if (!Bind())
                    return;

                _proxy = ZmqProxy.CreateAndRun(_ctx, _frontend, _backend, _capture);
            }
        }

        private bool Bind()
        {
            if (_socketBound)
                return true;

            ZError error;

            if (!_frontend.Bind(_config.FrontendAddress, out error))
            {
                _logger.Error($"Frontend socket of ReqRep proxy could not bind to {_config.FrontendAddress}. {error.Text}");
                return false;
            }

            if (!_backend.Bind(_config.BackendAddress, out error))
            {
                _logger.Error($"Backend socket of ReqRep proxy could not bind to {_config.BackendAddress}. {error.Text}");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(_config.CaptureAddress))
            {
                if (!_capture.Bind(_config.CaptureAddress, out error))
                {
                    _logger.Error($"Capture socket of ReqRep proxy could not bind to {_config.CaptureAddress}. {error.Text}");
                    return false;
                }
            }

            _socketBound = true;
            return true;
        }

        public void Dispose()
        {
            lock (_syncLock)
            {
                // Do NOT dispose the ctx. We don't own the context, only use it.
                _ctx = null;

                _proxy?.Dispose();
                _proxy = null;

                _capture?.Close();
                _capture?.Dispose();
                _capture = null;

                _frontend?.Close();
                _frontend?.Dispose();
                _frontend = null;

                _backend?.Close();
                _backend?.Dispose();
                _backend = null;

                _socketBound = false;
            }
        }
    }
}
