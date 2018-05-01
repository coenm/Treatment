namespace Treatment.Console
{
    using JetBrains.Annotations;

    public interface IRootDirSanitizer
    {
        void SetRootDir([NotNull] string input);

        string Sanitize(string input);
    }
}