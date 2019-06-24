namespace TestHelper
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public static class TestEnvironment
    {
        private const string RepositoryRoot = "REPOSITORY_ROOT";
        private static readonly Lazy<string> LazyRootDirectoryFullPath = new Lazy<string>(GetRootDirectoryFullPathImpl);
        private static readonly Lazy<bool> RunsOnContinuousIntegration = new Lazy<bool>(IsContinuousIntegrationImpl);
        private static readonly Lazy<bool> RunsOnContinuousIntegrationTravis = new Lazy<bool>(IsRunningOnTravisImpl);
        private static readonly Lazy<bool> RunsOnContinuousIntegrationAppVeyor = new Lazy<bool>(IsRunningOnAppVeyorImpl);
        private static readonly Lazy<bool> RunsOnContinuousIntegrationDevOps = new Lazy<bool>(IsRunningOnDevOpsImpl);

        /// <summary>
        /// Gets a value indicating whether test execution runs on CI.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static bool RunsOnCI => RunsOnContinuousIntegration.Value;

        public static bool RunsOnTravis => RunsOnContinuousIntegrationTravis.Value;

        public static bool RunsOnAppVeyor => RunsOnContinuousIntegrationAppVeyor.Value;

        public static bool RunsOnAzureDevOps => RunsOnContinuousIntegrationDevOps.Value;

        public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        private static string RootDirectoryFullPath => LazyRootDirectoryFullPath.Value;

        /// <summary>
        /// Convert relative path to full path based on solution directory.
        /// </summary>
        /// <param name="relativePath">relative path from root directory.</param>
        /// <returns>Full path.</returns>
        public static string GetFullPath(params string[] relativePath)
        {
            var paths = new[] { RootDirectoryFullPath }.Concat(relativePath).ToArray();
            return Path
                   .Combine(paths)
                   .Replace('\\', Path.DirectorySeparatorChar);
        }

        private static bool IsContinuousIntegrationImpl() => bool.TryParse(Environment.GetEnvironmentVariable("CI"), out var isCi) && isCi;

        private static bool IsRunningOnAppVeyorImpl() => bool.TryParse(Environment.GetEnvironmentVariable("APPVEYOR"), out var value) && value;

        // note that this env variable is not set by default. I've set this one in my azure-pipelines
        private static bool IsRunningOnDevOpsImpl() => bool.TryParse(Environment.GetEnvironmentVariable("AZURE_DEVOPS"), out var value) && value;

        private static bool IsRunningOnTravisImpl()
        {
            return bool.TryParse(Environment.GetEnvironmentVariable("TRAVIS"), out var value) && value;
        }

        private static string GetRootDirectoryFullPathImpl()
        {
            try
            {
                if (RunsOnAppVeyor)
                    return GetRootDirectoryAppVeyorFullPathImpl();

                if (RunsOnAzureDevOps)
                    return GetRootDirectoryAzureDevOpsFullPathImpl();
            }
            catch (Exception)
            {
                // swallow
            }

            return GetRootDirectoryLocalFullPathImpl();
        }

        private static string GetRootDirectoryAppVeyorFullPathImpl()
        {
            var envKey = "APPVEYOR_BUILD_FOLDER";

            var appveyorBuildFolder = Environment.GetEnvironmentVariable(envKey);
            if (string.IsNullOrWhiteSpace(appveyorBuildFolder))
                throw new NullReferenceException($"No directory found in env variable '{envKey}'");

            var directory = new DirectoryInfo(appveyorBuildFolder);

            while (!directory.EnumerateFiles(RepositoryRoot).Any())
            {
                try
                {
                    directory = directory.Parent;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Unable to find root directory from '{appveyorBuildFolder}' because of {ex.GetType().Name}!", ex);
                }

                if (directory == null)
                    throw new Exception($"Unable to find root directory from '{appveyorBuildFolder}'!");
            }

            return directory.FullName;
        }

        private static string GetRootDirectoryAzureDevOpsFullPathImpl()
        {
            var envKey = "Build.SourcesDirectory";

            var appveyorBuildFolder = Environment.GetEnvironmentVariable(envKey);
            if (string.IsNullOrWhiteSpace(appveyorBuildFolder))
            {
                envKey = "BuildSourcesDirectory";
                appveyorBuildFolder = Environment.GetEnvironmentVariable(envKey);
            }

            if (string.IsNullOrWhiteSpace(appveyorBuildFolder))
            {
                envKey = "Build_SourcesDirectory";
                appveyorBuildFolder = Environment.GetEnvironmentVariable(envKey);
            }

            if (string.IsNullOrWhiteSpace(appveyorBuildFolder))
                throw new NullReferenceException($"No directory found in env variable '{envKey}'");

            var directory = new DirectoryInfo(appveyorBuildFolder);

            while (!directory.EnumerateFiles(RepositoryRoot).Any())
            {
                try
                {
                    directory = directory.Parent;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Unable to find root directory from '{appveyorBuildFolder}' because of {ex.GetType().Name}!", ex);
                }

                if (directory == null)
                    throw new Exception($"Unable to find root directory from '{appveyorBuildFolder}'!");
            }

            return directory.FullName;
        }

        private static string GetRootDirectoryLocalFullPathImpl()
        {
            var assemblyLocation = typeof(TestEnvironment).GetTypeInfo().Assembly.Location;

            var assemblyFile = new FileInfo(assemblyLocation);

            var directory = assemblyFile.Directory;

            if (directory == null)
                throw new Exception($"Unable to find root directory from '{assemblyLocation}'!");

            while (!directory.EnumerateFiles(RepositoryRoot).Any())
            {
                try
                {
                    directory = directory.Parent;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Unable to find root directory from '{assemblyLocation}' because of {ex.GetType().Name}!", ex);
                }

                if (directory == null)
                    throw new Exception($"Unable to find root directory from '{assemblyLocation}'!");
            }

            return directory.FullName;
        }
    }
}
