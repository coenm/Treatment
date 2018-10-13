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
    using Treatment.UI.View;
    using Treatment.UI.ViewModel.Settings;

    using ICommand = System.Windows.Input.ICommand;

    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        [NotNull] private readonly ICommandHandler<UpdateProjectFilesCommand> _handlerUpdateProjectFilesCommand;
        [NotNull] private readonly ICommandHandler<CleanAppConfigCommand> _handlerCleanAppConfigCommand;
        [NotNull] private readonly IFileSearch _fileSearch;
        [NotNull] private readonly IConfiguration _configuration;
        [NotNull] private readonly IProgress<ProgressData> _progressFixCsProjectFiles;
        private string _workingDirectory;
        private string _fixCsProjectFilesLog;
        private ProgressData? _inProgress;


        public MainWindowViewModel(
            [NotNull] ICommandHandler<UpdateProjectFilesCommand> handlerUpdateProjectFilesCommand,
            [NotNull] ICommandHandler<CleanAppConfigCommand> handlerCleanAppConfigCommand,
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

            _handlerUpdateProjectFilesCommand = handlerUpdateProjectFilesCommand ?? throw new ArgumentNullException(nameof(handlerUpdateProjectFilesCommand));
            _handlerCleanAppConfigCommand = handlerCleanAppConfigCommand ?? throw new ArgumentNullException(nameof(handlerCleanAppConfigCommand));
            _fileSearch = fileSearch ?? throw new ArgumentNullException(nameof(fileSearch));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            WorkingDirectory = _configuration.RootPath ?? string.Empty;

            Sources = new ObservableCollection<ProjectViewModel>(CreateProjectViewModelsFromDirectory());

            OpenSettings = new OpenSettingsCommand();
        }

        private class OpenSettingsCommand : ICommand
        {
            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                var viewModel = new SettingsViewModel();
                var window = new SettingsWindow
                                 {
                                     DataContext = viewModel,
                                     // Owner = parentWindow,
                                 };

                var result = window.ShowDialog();

                // if (!result.HasValue || !result.Value)
                // {
                //     e.Result = false;
                //     return;
                // }
                //
                // e.Result = true;
                // e.ProcessType = viewModel.ProcessType;
                // e.Name = viewModel.ItemName;
                // e.ExecutablePath = viewModel.ExecutablePath;
                // e.Commandline = viewModel.Commandline;
                // e.ProjectPath = viewModel.ProjectPath;
                // e.BuildConfiguration = viewModel.BuildConfiguration;
                // e.BuildPlatform = viewModel.BuildPlatform;
            }


            public event EventHandler CanExecuteChanged;
        }

        public ObservableCollection<ProjectViewModel> Sources { get; }

        public ICommand OpenSettings { get; }

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

        public ProgressData? InProgress
        {
            get => _inProgress;
            set
            {
                if (_inProgress == null && value == null)
                    return;
                _inProgress = value;
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