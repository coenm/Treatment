namespace TestAgent.ZeroMq.Utils
{
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal static class ZmqConnection
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GiveZeroMqTimeToFinishConnectOrBind()
        {
            // We (need to) delay here fore 1ms. Don't know why but it seems that zmq is not ready yet
            // This is also done in the sample code of zmq sometimes:
            // see for instance: http://zguide.zeromq.org/cs:syncsub
            //
            // I didn't create an async variant with await Task.Delay(1)
            // because i figured that the overhead of constructing the
            // 'async statemachine' was more expensive than blocking
            // the thread for 1ms.
            Thread.Sleep(1);
        }
    }
}
