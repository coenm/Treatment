﻿namespace Treatment.TestAutomation.TestRunner.Framework.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITestAgent : IDisposable
    {
        Task<List<string>> LocateFilesAsync(string directory, string filename);

        Task<byte[]> GetFileContentAsync(string filename);

        Task<bool> StartSutAsync();

        Task<string> LocateSutExecutableAsync();

        Task<bool> DeleteFileAsync(string filename);

        Task<bool> FileExistsAsync(string filename);
    }
}
