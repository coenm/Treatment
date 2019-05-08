namespace Treatment.TestAutomation.TestRunner.Framework
{
    using System.IO;
    using System.Reflection;

    using JetBrains.Annotations;

    internal static class TestHelper
    {
        [CanBeNull]
        public static string GetSolutionDirectory()
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
