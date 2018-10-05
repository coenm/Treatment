namespace Treatment.UI.ViewModel
{
    using System;

    using JetBrains.Annotations;

    using Nito.Mvvm;

    using Treatment.Contract;
    using Treatment.Contract.Commands;

    public class MainWindowViewModel : IMainWindowViewModel
    {
        private readonly ICommandHandler<UpdateProjectFilesCommand> _commandHandler;

        public MainWindowViewModel([NotNull] ICommandHandler<UpdateProjectFilesCommand> commandHandler)
        {
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            Name = "test";

            FixCsProjectFiles = new AsyncCommand(async () =>
                                                 {
                                                     try
                                                     {
                                                         await commandHandler.ExecuteAsync(new UpdateProjectFilesCommand(""));
                                                     }
                                                     catch (Exception e)
                                                     {
                                                         // do nothing;
                                                         e = e;
                                                     }
                                                 });

            // FixCsProjectFiles = new AsyncCommand(async _ => {await TestService.DownloadAndCountBytesAsync(Url);});
        }

        public string Name { get; set; }

        public AsyncCommand FixCsProjectFiles { get; }
    }
}