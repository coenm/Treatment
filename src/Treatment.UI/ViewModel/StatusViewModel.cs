﻿namespace Treatment.UI.ViewModel
{
    using System;
    using System.Reactive.Linq;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Nito.Mvvm;
    using Treatment.Helpers;
    using Treatment.UI.Framework;
    using Treatment.UI.Framework.SynchronizationContext;
    using Treatment.UI.Framework.ViewModel;
    using Treatment.UI.Model;

    public class StatusViewModel : ViewModelBase, IStatusViewModel, IInitializableViewModel, IDisposable
    {
        [NotNull] private readonly object registerSyncLock = new object();
        [NotNull] private readonly IStatusModel statusModel;
        [NotNull] private readonly IUserInterfaceSynchronizationContextProvider uiContextProvider;
        [CanBeNull] private IDisposable observableModel;

        public StatusViewModel(
            [NotNull] IUserInterfaceSynchronizationContextProvider uiContextProvider,
            [NotNull] IStatusModel statusModel)
        {
            this.uiContextProvider = Guard.NotNull(uiContextProvider, nameof(uiContextProvider));
            this.statusModel = Guard.NotNull(statusModel, nameof(statusModel));

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

        System.Windows.Input.ICommand IInitializableViewModel.Initialize => Initialize;

        public CapturingExceptionAsyncCommand Initialize { get; }

        public void Dispose()
        {
            lock (registerSyncLock)
            {
                observableModel?.Dispose();
                observableModel = null;
            }
        }

        private void Register()
        {
            lock (registerSyncLock)
            {
                if (observableModel != null)
                    return;

                observableModel = Observable
                    .FromEventPattern(
                        handler => statusModel.Updated += handler,
                        handler => statusModel.Updated -= handler)
                    .ObserveOn(uiContextProvider.UiSynchronizationContext)
                    .Subscribe(data => UpdateData());

                uiContextProvider.UiSynchronizationContext.Post(_ => UpdateData(), null);
            }
        }

        private void UpdateData()
        {
            StatusText = statusModel.StatusText;
        }
    }
}
