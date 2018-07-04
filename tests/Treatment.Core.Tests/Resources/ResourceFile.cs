namespace Treatment.Core.Tests.Resources
{
    using System.IO;
    using System.Reflection;

    public static class ResourceFile
    {
        private static readonly string _embeddedResourceNs = typeof(ResourceFile).Namespace;
        private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();

        public static Stream OpenRead(string filename)
        {
            return _assembly.GetManifestResourceStream(_embeddedResourceNs + "." + filename);
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