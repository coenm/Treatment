namespace Treatment.UI.Model
{
    using System;

    public interface IStatusFullModel : IStatusReadModel
    {
        void UpdateStatus(string text);

        void SetConfigFilename(string filename);

        IDisposable NotifyDelay();
    }
}
