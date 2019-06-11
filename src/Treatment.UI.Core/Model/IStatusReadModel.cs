namespace Treatment.UI.Core.Model
{
    using System;

    public interface IStatusReadModel
    {
        event EventHandler Updated;

        /// <summary>
        /// Status text.
        /// </summary>
        string StatusText { get; }

        /// <summary>
        /// Configuration filename.
        /// </summary>
        string ConfigFilename { get; }

        /// <summary>
        /// Number of processes that are currently delayed.
        /// </summary>
        int DelayProcessCounter { get; }
    }
}
