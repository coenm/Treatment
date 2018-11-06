namespace Treatment.UI.Model
{
    using System;

    using JetBrains.Annotations;

    public class StatusModel : IStatusFullModel
    {
        private string statusText = string.Empty;
        private string configFilename = string.Empty;

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

        public void UpdateStatus(string text)
        {
            StatusText = text;
        }

        public void SetConfigFilename([NotNull] string filename)
        {
            ConfigFilename = filename;
        }
    }
}
