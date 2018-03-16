namespace Treatment.Core.UseCases.UpdateProjectFiles
{
    using System.Diagnostics;

    using Treatment.Core.UseCases;

    public class UpdateProjectFilesCommand : ITreatmentCommand
    {
        [DebuggerStepThrough]
        public UpdateProjectFilesCommand(string directory)
        {
            Directory = directory;
        }

        public string Directory { get; }
    }
}