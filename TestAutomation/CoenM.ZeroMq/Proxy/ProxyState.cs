namespace CoenM.ZeroMq.Proxy
{
    public enum ProxyState
    {
        /// <summary>
        /// Initialized.
        /// </summary>
        Initialized,

        /// <summary>
        /// Proxy is running.
        /// </summary>
        Running,

        /// <summary>
        /// Proxy is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// Proxy is terminated.
        /// </summary>
        Terminated,
    }
}
