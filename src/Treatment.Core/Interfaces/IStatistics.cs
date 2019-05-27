namespace Treatment.Core.Interfaces
{
    public interface IStatistics
    {
        void AddFileRead(string filename);

        void AddFileUpdate(string filename);

        void AddFoundFiles(string[] filenames);
    }
}