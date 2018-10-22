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

    public class UpdateProjectFilesCommandHandlerImplementation /*: ICommandHandler<UpdateProjectFilesCommand>*/
    {
        //todo don't use regex but xml serializer
        private const string SEARCH = @"<HintPath>[\.\.\\]+Packages\\(.+\.dll)</HintPath>";
        private const string REPLACE = @"<HintPath>$(PackagesDir)\$1</HintPath>";

        private readonly IFileSystem _filesystem;
        private readonly IFileSearch _fileSearcher;
        private readonly Regex _regex;

        public UpdateProjectFilesCommandHandlerImplementation([NotNull] IFileSystem filesystem, [NotNull] IFileSearch fileSearcher)
        {
            _filesystem = filesystem ?? throw new ArgumentNullException(nameof(filesystem));
            _fileSearcher = fileSearcher ?? throw new ArgumentNullException(nameof(fileSearcher));
            _regex = new Regex(SEARCH, RegexOptions.Compiled);
        }

        public Task ExecuteAsync(UpdateProjectFilesCommand command, IProgress<ProgressData> progress = null, CancellationToken ct = default(CancellationToken))
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
            return _fileSearcher.FindFilesIncludingSubdirectories(rootpath, "*.csproj");
        }

        public void FixSingleFile(string filename)
        {
            var originalContent = _filesystem.GetFileContent(filename);

            var updatedContent = FixContent(originalContent);

            if (originalContent.Equals(updatedContent))
                return;

            _filesystem.SaveContent(filename, updatedContent);
        }

        private string FixContent(string data)
        {
            var result = data;
            var match = _regex.Match(result);
            while (match.Success)
            {
                result = _regex.Replace(data, REPLACE);
                match = _regex.Match(result);
            }

            return result;
        }
    }
}