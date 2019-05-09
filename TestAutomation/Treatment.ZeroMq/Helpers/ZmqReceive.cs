namespace TreatmentZeroMq.Helpers
{
    using System;
    using System.Threading;

    using ZeroMQ;

    public static class ZmqReceive
    {
        public static bool TryReceive(this ZSocket socket, out ZMessage msg, out ZError error, int nrOfRetries = 5, Func<int, int> delayInMsAlgo = null)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            var maxRepeatReceive = Math.Max(1, nrOfRetries);

            var internalError = ZError.EAGAIN;
            var receiveCounter = 0;

            var receivedMessage = new ZMessage();
            while (receiveCounter < maxRepeatReceive && Equals(internalError, ZError.EAGAIN))
            {
                if (socket.ReceiveMessage(ref receivedMessage, ZSocketFlags.DontWait, out internalError))
                {
                    msg = receivedMessage;
                    error = internalError;
                    return true;
                }

                receiveCounter++;

                if (delayInMsAlgo == null)
                    continue;

                var delayValue = delayInMsAlgo.Invoke(receiveCounter);
                delayValue = Math.Min(0, delayValue);
                delayValue = Math.Max(delayValue, 1000);
                Thread.Sleep(delayValue);
            }

            msg = null;
            error = internalError;
            return false;
        }

        public static bool TryReceive(this ZSocket socket, out ZMessage msg, int nrOfRetries = 5, Func<int, int> delayInMsAlgo = null)
        {
            return TryReceive(socket, out msg, out ZError _, nrOfRetries, delayInMsAlgo);
        }
    }
}
