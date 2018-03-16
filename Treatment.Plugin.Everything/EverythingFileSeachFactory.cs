namespace Treatment.Plugin.Everything
{
    using System;

    using Treatment.Core.Interfaces;

    public class EverythingFileSeachFactory : ISearchProviderFactory
    {
        public int Priority { get; } = 1;

        public string Name { get; } = "Everything";

        public bool CanCreate(string name)
        {
            if (name == null)
                return false;

            if (name.Equals(Name, StringComparison.InvariantCultureIgnoreCase))
                return true;

            return false;
        }

        public IFileSearch Create()
        {
            return new EverythingFileSeachAdapter();
        }
    }
}