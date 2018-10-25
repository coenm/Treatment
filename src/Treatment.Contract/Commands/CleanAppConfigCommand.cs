namespace Treatment.Contract.Commands
{
    using JetBrains.Annotations;

    [PublicAPI]
    public class CleanAppConfigCommand : IDirectoryProperty, ICommand
    {
        public CleanAppConfigCommand(string directory)
        {
            Directory = directory;
        }

        public string Directory { get; }
    }
}
