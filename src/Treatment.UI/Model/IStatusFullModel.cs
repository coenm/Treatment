namespace Treatment.UI.Model
{
    public interface IStatusFullModel : IStatusReadModel
    {
        void UpdateStatus(string text);

        void SetConfigFilename(string filename);
    }
}
