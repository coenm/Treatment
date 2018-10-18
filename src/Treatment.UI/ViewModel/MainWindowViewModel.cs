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
    using Treatment.UI.Core.UI;
    using Treatment.UI.ViewModel.Settings;

    using ICommand = System.Windows.Input.ICommand;

    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        [NotNull] private readonly ICommandHandler<UpdateProjectFilesCommand> _handlerUpdateProjectFilesCommand;
        [NotNull] private readonly ICommandHandler<CleanAppConfigCommand> _handlerCleanAppConfigCommand;
        [NotNull] private readonly IFileSearch _fileSearch;
        [NotNull] private readonly IConfiguration _configuration;
        [NotNull] private readonly IProgress<ProgressData> _progressFixCsProjectFiles;

        public MainWindowViewModel(
            [NotNull] ICommandHandler<UpdateProjectFilesCommand> handlerUpdateProjectFilesCommand,
            [NotNull] ICommandHandler<CleanAppConfigCommand> handlerCleanAppConfigCommand,
            [NotNull] IFileSearch fileSearch,
            [NotNull] IConfiguration configuration,
            [NotNull] IShowEntityInDialogProcessor showInDialogProcessor)
        {
            _progressFixCsProjectFiles = new Progress<ProgressData>(data =>
                                                                    {
                                                                        if (string.IsNullOrEmpty(data.Message))
                                                                            return;

                                                                        // THIS IS PROBABLY NOT THE WAY TO DO THIS..
                                                                        FixCsProjectFilesLog += data.Message + Environment.NewLine;
                                                                    });

            _handlerUpdateProjectFilesCommand = handlerUpdateProjectFilesCommand ?? throw new ArgumentNullException(nameof(handlerUpdateProjectFilesCommand));
            _handlerCleanAppConfigCommand = handlerCleanAppConfigCommand ?? throw new ArgumentNullException(nameof(handlerCleanAppConfigCommand));
            _fileSearch = fileSearch ?? throw new ArgumentNullException(nameof(fileSearch));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            if (showInDialogProcessor == null)
                throw new ArgumentNullException(nameof(showInDialogProcessor));

            WorkingDirectory = _configuration.RootPath ?? string.Empty;

            Sources = new ObservableCollection<ProjectViewModel>(CreateProjectViewModelsFromDirectory());

            OpenSettings = new OpenSettingsCommand(showInDialogProcessor, _configuration.RootPath ?? string.Empty);
        }

        public ObservableCollection<ProjectViewModel> Sources { get; }

        public ICommand OpenSettings { get; }

        public string FixCsProjectFilesLog
        {
            get => Properties.Get(string.Empty);
            set => Properties.Set(value);
        }

        public string WorkingDirectory
        {
            get => Properties.Get(string.Empty);
            set => Properties.Set(value);
        }

        public ProgressData? InProgress
        {
            get => Properties.Get<ProgressData>(null);
            set => Properties.Set(value);
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

        private class OpenSettingsCommand : ICommand
        {
            private readonly IShowEntityInDialogProcessor _showEntityInDialogProcessor;
            [NotNull] private readonly string _rootPath;

            public OpenSettingsCommand([NotNull] IShowEntityInDialogProcessor showEntityInDialogProcessor, [NotNull] string rootPath)
            {
                _showEntityInDialogProcessor = showEntityInDialogProcessor ?? throw new ArgumentNullException(nameof(showEntityInDialogProcessor));
                _rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                var applicationSettings = new ApplicationSettings
                                              {
                                                  DelayExecution = true,
                                                  SearchProviderName = "FileSystem",
                                                  VersionControlProviderName = "SVN",
                                                  RootDirectory = _rootPath
                                              };

                var result = _showEntityInDialogProcessor.ShowDialog(applicationSettings);
                if (result == true)
                {
                }
            }
        }
    }
}