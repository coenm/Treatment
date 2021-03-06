﻿namespace CoenM.ZeroMq.Helpers
{
    using ZeroMQ;

    public static class ZmqConnect
    {
        public static bool TryConnect(this ZSocket socket, string endpoint)
        {
            if (!socket.Connect(endpoint, out _))
            {
                // Error(Logger, "ZeroMq could not connect", error);
                return false;
            }

            return true;
        }

        public static bool TryBind(this ZSocket socket, string endpoint)
        {
            if (!socket.Bind(endpoint, out _))
            {
                // Error(Logger, "ZeroMq could not bind", error);
                return false;
            }

            return true;
        }
    }
}
