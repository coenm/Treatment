namespace Treatment.UI.Core.Model
{
    using System;
    using System.Threading;
    using JetBrains.Annotations;

    public class StatusModel : IStatusFullModel
    {
        private string statusText = string.Empty;
        private string configFilename = string.Empty;
        private int delayProcessCounter;

        public event EventHandler Updated;

        public string StatusText
        {
            get => statusText;

            private set
            {
                statusText = value;
                Updated?.Invoke(this, EventArgs.Empty);
            }
        }

        public string ConfigFilename
        {
            get => configFilename;

            private set
            {
                configFilename = value;
                Updated?.Invoke(this, EventArgs.Empty);
            }
        }

        public int DelayProcessCounter => delayProcessCounter;

        public void UpdateStatus(string text)
        {
            StatusText = text;
        }

        public void SetConfigFilename([NotNull] string filename)
        {
            ConfigFilename = filename;
        }

        public IDisposable NotifyDelay()
        {
            Interlocked.Increment(ref delayProcessCounter);
            Updated?.Invoke(this, EventArgs.Empty);

            return new InvokeActionOnDisposal(() =>
            {
                Interlocked.Decrement(ref delayProcessCounter);
                Updated?.Invoke(this, EventArgs.Empty);
            });
        }

        private class InvokeActionOnDisposal : IDisposable
        {
            private readonly Action actionToInvokeOnDisposal;

            public InvokeActionOnDisposal([NotNull] Action actionToInvokeOnDisposal)
            {
                this.actionToInvokeOnDisposal = actionToInvokeOnDisposal;
            }

            public void Dispose()
            {
                actionToInvokeOnDisposal.Invoke();
            }
        }
    }
}
