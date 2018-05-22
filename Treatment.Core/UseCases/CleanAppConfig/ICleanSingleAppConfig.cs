namespace Treatment.Core.UseCases.CleanAppConfig
{
    public interface ICleanSingleAppConfig
    {
        bool Execute(string projectFile, string appConfigFile);
    }
}