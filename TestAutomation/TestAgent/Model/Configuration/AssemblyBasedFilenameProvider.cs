namespace TestAgent.Model.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;

    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Unittest purposes")]
    internal class AssemblyBasedFilenameProvider : IConfigFilenameProvider
    {
        public string Filename
        {
            get
            {
                const string filename = "TestAgentConfig.yaml";
                var assemblyFullFilename = GetAssemblyLocation();

                var basePath = assemblyFullFilename;
                if (string.IsNullOrEmpty(assemblyFullFilename))
                    basePath = Environment.CurrentDirectory;
                else
                    basePath = Path.GetDirectoryName(assemblyFullFilename);

                return Path.Combine(basePath, filename);
            }
        }

        protected virtual string GetAssemblyLocation()
        {
            var assembly = Assembly.GetEntryAssembly() ?? typeof(AssemblyBasedFilenameProvider).Assembly;
            return assembly.Location;
        }
    }
}
