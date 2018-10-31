namespace Treatment.UI.Model
{
    using System;

    public class StatusModel : IStatusModel
    {
        private string statusText = string.Empty;

        public event EventHandler Updated;

        public string StatusText
        {
            get => statusText;

            set
            {
                statusText = value;
                Updated?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
