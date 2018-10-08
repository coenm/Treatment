namespace Treatment.UI.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    using Nito.Mvvm;

    using Treatment.Contract;
    using Treatment.Contract.Commands;

    public class MainWindowViewModel : IMainWindowViewModel, INotifyPropertyChanged
    {
        private readonly ICommandHandler<UpdateProjectFilesCommand> _commandHandler;
        private string _workingDirectory;
        private IProgress<ProgressData> _progressFixCsProjectFiles;
        private string _fixCsProjectFilesLog;

        public MainWindowViewModel([NotNull] ICommandHandler<UpdateProjectFilesCommand> commandHandler)
        {
            _progressFixCsProjectFiles = new Progress<ProgressData>(data =>
                                                                    {
                                                                        if (string.IsNullOrEmpty(data.Message))
                                                                            return;

                                                                        // THIS IS PROBABLY NOT THE WAY TO DO THIS..
                                                                        FixCsProjectFilesLog += data.Message + Environment.NewLine;
                                                                    });
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            WorkingDirectory = "D:\\Users\\coen\\Downloads\\temp";

            FixCsProjectFiles = new CapturingExceptionAsyncCommand(async _ => await commandHandler.ExecuteAsync(new UpdateProjectFilesCommand(WorkingDirectory), _progressFixCsProjectFiles));
            // FixCsProjectFiles = new CapturingExceptionAsyncCommand(async _ => await commandHandler.ExecuteAsync(new UpdateProjectFilesCommand("D:\\Users\\coen\\Downloads\\temp\\")));
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

        public CapturingExceptionAsyncCommand FixCsProjectFiles { get; }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}