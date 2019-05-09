namespace Treatment.TestAutomation.TestRunner.Sut
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITestAgent : IDisposable
    {
        // todo this one should be shared with other project.

        Task<List<string>> LocateFilesAsync(string directory, string filename);

        Task<byte[]> GetFileContentAsync(string filename);

        Task<bool> StartSutAsync();

        Task<string> LocateSutExecutableAsync();
    }
}
