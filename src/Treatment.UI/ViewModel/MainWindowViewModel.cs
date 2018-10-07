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

        public MainWindowViewModel([NotNull] ICommandHandler<UpdateProjectFilesCommand> commandHandler)
        {
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            Name = "test";

            FixCsProjectFiles = new CapturingExceptionAsyncCommand(async _ => await commandHandler.ExecuteAsync(new UpdateProjectFilesCommand("D:\\Users\\coen\\Downloads\\temp\\")));

        }

        public string Name { get; set; }

        public CapturingExceptionAsyncCommand FixCsProjectFiles { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}