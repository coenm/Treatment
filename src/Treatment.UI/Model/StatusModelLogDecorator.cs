namespace Treatment.UI.Model
{
    using System;

    using JetBrains.Annotations;
    using NLog;
    using Treatment.Helpers.Guards;

    [UsedImplicitly]
    public class StatusModelLogDecorator : IStatusFullModel
    {
        [NotNull] private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        [NotNull] private readonly IStatusFullModel decoratee;

        public StatusModelLogDecorator([NotNull] IStatusFullModel decoratee)
        {
            Guard.NotNull(decoratee, nameof(decoratee));
            this.decoratee = decoratee;
        }

        public event EventHandler Updated
        {
            add => decoratee.Updated += value;
            remove => decoratee.Updated -= value;
        }

        public string StatusText => decoratee.StatusText;

        public string ConfigFilename => decoratee.ConfigFilename;

        public int DelayProcessCounter => decoratee.DelayProcessCounter;

        public void UpdateStatus(string text)
        {
            Logger.Debug(() => $"Set UpdateStatus: {text}");
            decoratee.UpdateStatus(text);
        }

        public void SetConfigFilename(string filename)
        {
            Logger.Debug(() => $"Set configuration filename {filename}");
            decoratee.SetConfigFilename(filename);
        }

        public IDisposable NotifyDelay()
        {
            Logger.Debug(() => "Starting a delay");
            return decoratee.NotifyDelay();
        }
    }
}
