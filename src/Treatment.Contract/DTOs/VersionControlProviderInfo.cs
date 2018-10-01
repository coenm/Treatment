namespace Treatment.Contract.DTOs
{
    using JetBrains.Annotations;

    public class VersionControlProviderInfo
    {
        public VersionControlProviderInfo(int priority, [NotNull] string name)
        {
            Priority = priority;
            Name = name;
        }

        public string Name { get; }

        public int Priority { get; }
    }
}