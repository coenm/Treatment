namespace Treatment.Core.UseCases.CleanAppConfig
{
    using System.Threading.Tasks;

    public interface ICleanSingleAppConfig
    {
        Task<bool> ExecuteAsync(string projectFile, string appConfigFile);
    }
}