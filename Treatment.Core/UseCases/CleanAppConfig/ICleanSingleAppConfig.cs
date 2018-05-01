namespace Treatment.Core.UseCases.CleanAppConfig
{
    public interface ICleanSingleAppConfig
    {
        void Execute(string projectFile, string appConfigFile);
    }
}