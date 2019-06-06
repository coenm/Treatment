namespace Treatment.UI.Implementations.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;

    using JetBrains.Annotations;
    using Treatment.Helpers.File;
    using Treatment.Helpers.Guards;

    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Unittest purposes")]
    internal class VerifyAndFixFilenameDecorator : IConfigFilenameProvider
    {
        [NotNull] private readonly IConfigFilenameProvider decoratee;

        public VerifyAndFixFilenameDecorator([NotNull] IConfigFilenameProvider decoratee)
        {
            Guard.NotNull(decoratee, nameof(decoratee));
            this.decoratee = decoratee;
        }

        public string Filename
        {
            get
            {
                var filename = decoratee.Filename;

                if (FileHelper.IsAbsoluteValidPath(filename))
                {
                    // No problem if the file doesn't exist yet.
                    return filename;
                }

                var assemblyFullFilename = GetAssemblyLocation();

                var basePath = assemblyFullFilename;
                if (string.IsNullOrEmpty(assemblyFullFilename))
                    basePath = Environment.CurrentDirectory;
                else
                    basePath = Path.GetDirectoryName(assemblyFullFilename);

                var newConfigFilename = Path.Combine(basePath, filename);

                if (FileHelper.IsAbsoluteValidPath(newConfigFilename))
                    return newConfigFilename;

                return Path.Combine(basePath, "TreatmentApplicationConfiguration.txt");
            }
        }

        protected virtual string GetAssemblyLocation()
        {
            var assembly = Assembly.GetEntryAssembly() ?? typeof(VerifyAndFixFilenameDecorator).Assembly;
            return assembly.Location;
        }
    }
}
