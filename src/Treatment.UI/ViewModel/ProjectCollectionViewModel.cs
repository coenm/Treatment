namespace Treatment.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using CoenM.Encoding;

    using JetBrains.Annotations;

    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.UI.Core;

    public class ProjectCollectionViewModel : ViewModelBase, IDisposable
    {
        [NotNull]
        private readonly ICommandHandler<UpdateProjectFilesCommand> _handlerUpdateProjectFilesCommand;

        [NotNull]
        private readonly ICommandHandler<CleanAppConfigCommand> _handlerCleanAppConfigCommand;

        [NotNull]
        private readonly IFileSearch _fileSearch;

        [NotNull]
        private readonly IConfiguration _configuration;

        public ProjectCollectionViewModel(
            [NotNull] ICommandHandler<UpdateProjectFilesCommand> handlerUpdateProjectFilesCommand,
            [NotNull] ICommandHandler<CleanAppConfigCommand> handlerCleanAppConfigCommand,
            [NotNull] IFileSearch fileSearch,
            [NotNull] IConfiguration configuration)
        {
            _handlerUpdateProjectFilesCommand = handlerUpdateProjectFilesCommand ?? throw new ArgumentNullException(nameof(handlerUpdateProjectFilesCommand));
            _handlerCleanAppConfigCommand = handlerCleanAppConfigCommand ?? throw new ArgumentNullException(nameof(handlerCleanAppConfigCommand));
            _fileSearch = fileSearch ?? throw new ArgumentNullException(nameof(fileSearch));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            Projects = new ObservableCollection<ProjectViewModel>();

            var items = CreateProjectViewModelsFromDirectory();
            foreach (var item in items)
                Projects.Add(item);
        }

        public ObservableCollection<ProjectViewModel> Projects { get; }

        public void Dispose()
        {
        }

        [NotNull]
        private IEnumerable<ProjectViewModel> CreateProjectViewModelsFromDirectory()
        {
            var rootPath = _configuration.RootPath;
            if (rootPath == null)
                yield break;

            if (!Directory.Exists(rootPath))
                yield break;

            var files = _fileSearch.FindFilesIncludingSubdirectories(rootPath, "*.sln");

            foreach (var file in files)
            {
                DirectoryInfo rootDirectoryInfo;
                try
                {
                    var filename = Path.GetFileName(file);
                    if (filename == null)
                        continue;

                    // No need to expose the filename to look for at github ;-)
                    if (Hash(filename) != "fTABLb)<0:PI1+6/8C%b5gd>4nRK{6SerJz+C)ik")
                        continue;

                    var fullDirectory = Path.GetDirectoryName(file);
                    var folderName = Path.GetFileName(fullDirectory);

                    if (folderName != "sln")
                        continue;

                    rootDirectoryInfo = Directory.GetParent(fullDirectory);
                    if (rootDirectoryInfo == null)
                        continue;
                }
                catch (Exception)
                {
                    // swallow
                    continue;
                }

                yield return new ProjectViewModel(
                                                  rootDirectoryInfo.Name,
                                                  rootDirectoryInfo.FullName,
                                                  _handlerUpdateProjectFilesCommand,
                                                  _handlerCleanAppConfigCommand);
            }
        }

        private static string Hash([CanBeNull] string filename)
        {
            if (filename == null)
                return string.Empty;

            var crypt = new SHA256Managed();
            var result = crypt.ComputeHash(
                                           crypt.ComputeHash(
                                                             new byte[] { 0, 1, 3, 5, 7, 9 }
                                                                 .Concat(Encoding.ASCII.GetBytes(filename))
                                                                 .ToArray())
                                                .Concat(
                                                        new byte[] { 34, 22, 230, 33, 33, 0 })
                                                .ToArray());

            return Z85.Encode(result);
        }
    }
}