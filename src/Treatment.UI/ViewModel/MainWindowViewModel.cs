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

    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        [NotNull] private readonly ICommandHandler<UpdateProjectFilesCommand> _commandHandler;
        [NotNull] private readonly IFileSearch _fileSearch;
        [NotNull] private readonly IConfiguration _configuration;
        private string _workingDirectory;
        private IProgress<ProgressData> _progressFixCsProjectFiles;
        private string _fixCsProjectFilesLog;
        private ObservableCollection<ProjectViewModel> _sources;

        public MainWindowViewModel([NotNull] ICommandHandler<UpdateProjectFilesCommand> commandHandler,
                                   [NotNull] IFileSearch fileSearch,
                                   [NotNull] IConfiguration configuration)
        {
            _progressFixCsProjectFiles = new Progress<ProgressData>(data =>
                                                                    {
                                                                        if (string.IsNullOrEmpty(data.Message))
                                                                            return;

                                                                        // THIS IS PROBABLY NOT THE WAY TO DO THIS..
                                                                        FixCsProjectFilesLog += data.Message + Environment.NewLine;
                                                                    });

            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            _fileSearch = fileSearch ?? throw new ArgumentNullException(nameof(fileSearch));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            WorkingDirectory = _configuration.RootPath ?? string.Empty;

            Sources = new ObservableCollection<ProjectViewModel>(CreateProjectViewModelsFromDirectory());

        }

        public ObservableCollection<ProjectViewModel> Sources
        {
            get => _sources;
            private set
            {
                if (_sources == value)
                    return;
                _sources = value;
                OnPropertyChanged();
            }
        }

        public string FixCsProjectFilesLog
        {
            get => _fixCsProjectFilesLog;
            set
            {
                if (value == _fixCsProjectFilesLog)
                    return;
                _fixCsProjectFilesLog = value;
                OnPropertyChanged();
            }
        }

        public string WorkingDirectory
        {
            get => _workingDirectory;
            set
            {
                if (_workingDirectory == value)
                    return;
                _workingDirectory = value;
                OnPropertyChanged();
            }
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

                yield return new ProjectViewModel(rootDirectoryInfo.Name, rootDirectoryInfo.FullName, _commandHandler);
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
                                                        new byte[] {34, 22, 230, 33, 33, 0 })
                                                .ToArray());

            return Z85.Encode(result);
        }
    }
}