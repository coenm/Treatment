namespace TestAgent
{
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

    internal class ConventionBasedResolveSutExecutable : IResolveSutExecutable
    {
        private string executable;

        public string Executable
        {
            get
            {
                if (executable != null)
                    return executable;

                var slnDir = GetSolutionDirectory();
                if (slnDir == null)
                    return null;

                var foundFiles = Directory.GetFiles(
                    slnDir,
                    "Treatment.UIStart.exe",
                    SearchOption.AllDirectories);

                executable = foundFiles.FirstOrDefault(x => x.EndsWith("Treatment.UI.Start\\bin\\x64\\Debug\\Treatment.UIStart.exe"));

                if (executable == null)
                    executable = foundFiles.FirstOrDefault();

                return executable;
            }
        }

        [CanBeNull]
        private static string GetSolutionDirectory()
        {
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var slnDir = currentDir;

            if (slnDir == null)
                return null;

            while (!File.Exists(Path.Combine(slnDir, "Treatment.sln")) && slnDir.Length > 4)
            {
                slnDir = Path.GetFullPath(Path.Combine(slnDir, ".."));
            }

            return slnDir;
        }
    }
}
