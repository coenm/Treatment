namespace TestAgent.ZeroMq.Extensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using ZeroMQ;

    public static class ZmqSend
    {
        public static async Task<bool> TrySendAsync(this ZSocket socket, ZMessage msg, int nrOfRetries = 5, Func<int, int> delayInMsAlgo = null)
        {
            ZError er = await SendAsync(socket, msg, nrOfRetries, delayInMsAlgo).ConfigureAwait(false);
            return Equals(er, ZError.None);
        }

        public static async Task<ZError> SendAsync(this ZSocket socket, ZMessage msg, int nrOfRetries = 5, Func<int, int> delayInMsAlgo = null)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            if (msg == null)
                throw new ArgumentNullException(nameof(msg));

            if (delayInMsAlgo == null)
                delayInMsAlgo = DetermineDelay;

            var maxRepeatSend = Math.Max(1, nrOfRetries);

            var internalError = ZError.EAGAIN;
            var sendCounter = 0;

            while (sendCounter < maxRepeatSend && Equals(internalError, ZError.EAGAIN))
            {
                if (socket.SendMessage(msg, ZSocketFlags.DontWait, out internalError))
                    return ZError.None;

                sendCounter++;

                var delayValue = delayInMsAlgo.Invoke(sendCounter);
                delayValue = Math.Min(0, delayValue);
                delayValue = Math.Max(delayValue, 1000);
                await Task.Delay(delayValue).ConfigureAwait(false);
            }

            return internalError ?? ZError.None;
        }

        public static bool TrySend(this ZSocket socket, ZMessage msg, out ZError error, int nrOfRetries = 5, Func<int, int> delayInMsAlgo = null)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            if (msg == null)
                throw new ArgumentNullException(nameof(msg));


            if (delayInMsAlgo == null)
            {
                delayInMsAlgo = DetermineDelay;
            }

            var maxRepeatSend = Math.Max(1, nrOfRetries);

            var internalError = ZError.EAGAIN;
            var sendCounter = 0;

            while (sendCounter < maxRepeatSend && Equals(internalError, ZError.EAGAIN))
            {
                if (socket.SendMessage(msg, ZSocketFlags.DontWait, out internalError))
                {
                    error = internalError;
                    return true;
                }

                sendCounter++;

                var delayValue = delayInMsAlgo.Invoke(sendCounter);
                delayValue = Math.Min(0, delayValue);
                delayValue = Math.Max(delayValue, 1000);
                Thread.Sleep(delayValue);
            }

            error = internalError;
            return false;
        }

        public static bool TrySend(this ZSocket socket, ZMessage msg, int nrOfRetries = 5, Func<int, int> delayInMsAlgo = null)
        {
            ZError error;
            return TrySend(socket, msg, out error, nrOfRetries, delayInMsAlgo);
        }

        public static bool TrySend(this ZSocket socket, ZFrame frame, out ZError error, int nrOfRetries = 5, Func<int, int> delayInMsAlgo = null)
        {
            // duplicate the frame because i don't know what happens to the frame when it is added to message and disposed.
            ZFrame f = frame.Duplicate();

            using (var msg = new ZMessage(new[] { f }))
            {
                return TrySend(socket, msg, out error, nrOfRetries, delayInMsAlgo);
            }
        }

        public static bool TrySend(this ZSocket socket, ZFrame frame, int nrOfRetries = 5, Func<int, int> delayInMsAlgo = null)
        {
            ZError error;
            return TrySend(socket, frame, out error, nrOfRetries, delayInMsAlgo);
        }

        /// <summary>Try to send a single, empty ZFrame to the socket.</summary>
        /// <param name="socket">ZSocket to send the empty message to</param>
        /// <param name="nrOfRetries">Number of send tries.</param>
        /// <param name="delayInMsAlgo">Optional algorithm to determine the wait before the next retry in milliseconds.</param>
        /// <returns></returns>
        public static bool TryPoke(this ZSocket socket, int nrOfRetries = 5, Func<int, int> delayInMsAlgo = null)
        {
            ZError error;
            using (var frame = new ZFrame())
            {
                return TrySend(socket, frame, out error, nrOfRetries, delayInMsAlgo);
            }
        }

        private static int DetermineDelay(int retryCounter)
        {
            return retryCounter * 50;
        }
    }
}
