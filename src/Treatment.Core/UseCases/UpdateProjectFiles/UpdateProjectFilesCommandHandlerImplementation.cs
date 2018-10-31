namespace Treatment.Core.UseCases.UpdateProjectFiles
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Core.Interfaces;
    using Treatment.Helpers;

    public class UpdateProjectFilesCommandHandlerImplementation /*: ICommandHandler<UpdateProjectFilesCommand>*/
    {
        // todo don't use regex but xml serializer
        private const string Search = @"<HintPath>[\.\.\\]+Packages\\(.+\.dll)</HintPath>";
        private const string Replace = @"<HintPath>$(PackagesDir)\$1</HintPath>";

        private readonly IFileSystem filesystem;
        private readonly IFileSearch fileSearcher;
        private readonly Regex regex;

        public UpdateProjectFilesCommandHandlerImplementation([NotNull] IFileSystem filesystem, [NotNull] IFileSearch fileSearcher)
        {
            this.filesystem = Guard.NotNull(filesystem, nameof(filesystem));
            this.fileSearcher = Guard.NotNull(fileSearcher, nameof(fileSearcher));
            regex = new Regex(Search, RegexOptions.Compiled);
        }

        public Task ExecuteAsync(UpdateProjectFilesCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default)
        {
            var files = GetCsFiles(command.Directory);
            foreach (var file in files)
            {
                FixSingleFile(file);
            }

            return Task.CompletedTask;
        }

        public string[] GetCsFiles(string rootpath)
        {
            return fileSearcher.FindFilesIncludingSubdirectories(rootpath, "*.csproj");
        }

        public void FixSingleFile(string filename)
        {
            var originalContent = filesystem.GetFileContent(filename);

            var updatedContent = FixContent(originalContent);

            if (originalContent.Equals(updatedContent))
                return;

            filesystem.SaveContent(filename, updatedContent);
        }

        private string FixContent(string data)
        {
            var result = data;
            var match = regex.Match(result);
            while (match.Success)
            {
                result = regex.Replace(data, Replace);
                match = regex.Match(result);
            }

            return result;
        }
    }
}
