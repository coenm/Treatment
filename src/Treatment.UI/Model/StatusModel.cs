namespace Treatment.UI.Model
{
    using System;

    public class StatusModel : IStatusFullModel
    {
        private string statusText = string.Empty;

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

        public void UpdateStatus(string text)
        {
            StatusText = text;
        }
    }
}
