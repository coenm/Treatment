namespace Treatment.Core.Tests.Resources
{
    using System.IO;
    using System.Reflection;

    public static class ResourceFile
    {
        private static readonly string EmbeddedResourceNs = typeof(ResourceFile).Namespace;
        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

        public static Stream OpenRead(string filename)
        {
            return Assembly.GetManifestResourceStream(EmbeddedResourceNs + "." + filename);
        }

        public static string GetContent(string filename)
        {
            using (var stream = OpenRead(filename))
            using (var sr = new StreamReader(stream))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
