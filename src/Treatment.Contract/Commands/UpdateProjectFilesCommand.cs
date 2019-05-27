namespace Treatment.Contract.Commands
{
    using System.Diagnostics;

    using JetBrains.Annotations;

    using Treatment.Contract;

    [PublicAPI]
    public class UpdateProjectFilesCommand : IDirectoryProperty, ICommand
    {
        [DebuggerStepThrough]
        public UpdateProjectFilesCommand([NotNull] string directory)
        {
            Directory = directory;
        }

        public string Directory { get; }
    }
}
