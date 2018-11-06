namespace Treatment.UI.ViewModel
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Nito.Mvvm;

    using Treatment.Helpers.Guards;
    using Treatment.UI.Framework.SynchronizationContext;
    using Treatment.UI.Framework.ViewModel;
    using Treatment.UI.Model;

    public class StatusViewModel : ViewModelBase, IStatusViewModel, IInitializableViewModel, IDisposable
    {
        [NotNull] private readonly object registerSyncLock = new object();
        [NotNull] private readonly IStatusReadModel statusModel;
        [NotNull] private readonly IUserInterfaceSynchronizationContextProvider uiContextProvider;
        [NotNull] private readonly CompositeDisposable observableModel;
        private bool registered;

        public StatusViewModel(
            [NotNull] IUserInterfaceSynchronizationContextProvider uiContextProvider,
            [NotNull] IStatusReadModel statusModel)
        {
            Guard.NotNull(uiContextProvider, nameof(uiContextProvider));
            Guard.NotNull(statusModel, nameof(statusModel));

            this.uiContextProvider = uiContextProvider;
            this.statusModel = statusModel;
            observableModel = new CompositeDisposable(1);
            registered = false;
            Initialize = new CapturingExceptionAsyncCommand(_ =>
            {
                Register();
                return Task.CompletedTask;
            });
        }

        public string StatusText
        {
            get => Properties.Get(statusModel.StatusText);
            private set => Properties.Set(value);
        }

        public string ConfigFilename
        {
            get => Properties.Get(statusModel.ConfigFilename);
            private set => Properties.Set(value);
        }

        public int DelayProcessCounter
        {
            get => Properties.Get(statusModel.DelayProcessCounter);
            private set => Properties.Set(value);
        }

        System.Windows.Input.ICommand IInitializableViewModel.Initialize => Initialize;

        public CapturingExceptionAsyncCommand Initialize { get; }

        public void Dispose()
        {
            lock (registerSyncLock)
            {
                observableModel.Dispose();
                observableModel.Clear();
                registered = false;
            }
        }

        private void Register()
        {
            lock (registerSyncLock)
            {
                if (registered)
                    return;

                observableModel.Add(Observable
                    .FromEventPattern(
                        handler => statusModel.Updated += handler,
                        handler => statusModel.Updated -= handler)
                    .ObserveOn(uiContextProvider.UiSynchronizationContext)
                    .Subscribe(data => UpdateData()));

                registered = true;
            }

            uiContextProvider.UiSynchronizationContext.Post(_ => UpdateData(), null);
        }

        private void UpdateData()
        {
            StatusText = statusModel.StatusText;
            ConfigFilename = statusModel.ConfigFilename;
            DelayProcessCounter = statusModel.DelayProcessCounter;
        }
    }
}
