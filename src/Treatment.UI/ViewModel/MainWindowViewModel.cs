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

        public MainWindowViewModel([NotNull] ICommandHandler<UpdateProjectFilesCommand> commandHandler)
        {
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            WorkingDirectory = "D:\\Users\\coen\\Downloads\\temp";

            FixCsProjectFiles = new CapturingExceptionAsyncCommand(async _ => await commandHandler.ExecuteAsync(new UpdateProjectFilesCommand(WorkingDirectory)));
            // FixCsProjectFiles = new CapturingExceptionAsyncCommand(async _ => await commandHandler.ExecuteAsync(new UpdateProjectFilesCommand("D:\\Users\\coen\\Downloads\\temp\\")));
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