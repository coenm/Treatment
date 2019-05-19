namespace Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations
{
    using System;
    using System.Collections.Concurrent;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using JetBrains.Annotations;
    using Treatment.TestAutomation.Contract.Interfaces.Events;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;
    using Treatment.TestAutomation.TestRunner.Controls.Framework;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    /// <summary>
    /// Managing/storing the creation and destruction of remote objects based on the events..
    /// </summary>
    public class RemoteObjectManager : IDisposable
    {
        [CanBeNull] private ITreatmentApplication application;
        [NotNull] private readonly CompositeDisposable disposable;
        [NotNull] private readonly ConcurrentDictionary<Guid, object> store;

        public RemoteObjectManager([NotNull] IApplicationEvents applicationEvents)
        {
            Treatment.Helpers.Guards.Guard.NotNull(applicationEvents, nameof(applicationEvents));

            store = new ConcurrentDictionary<Guid, object>();

            disposable = new CompositeDisposable
            {
                applicationEvents.Events
                    .Where(x => x is NewControlCreated)
                    .Subscribe(ev =>
                    {
                        var e = (NewControlCreated)ev;

                        if (e.InterfaceType == typeof(IButton).FullName)
                        {
                            store.TryAdd(e.Guid, new RemoteButton(e.Guid, applicationEvents));
                        }
                        else if (e.InterfaceType == typeof(ITextBlock).FullName)
                        {
                            store.TryAdd(e.Guid, new RemoteTextBlock(e.Guid, applicationEvents));
                        }
                        else if (e.InterfaceType == typeof(IMainViewStatusBar).FullName)
                        {
                            store.TryAdd(e.Guid, new RemoteMainViewStatusBar(e.Guid, applicationEvents, this));
                        }
                        else if (e.InterfaceType == typeof(ISettingWindow).FullName)
                        {
                            store.TryAdd(e.Guid, new RemoteSettingWindow(e.Guid, applicationEvents, this));
                        }
                        else if (e.InterfaceType == typeof(IMainWindow).FullName)
                        {
                            store.TryAdd(e.Guid, new RemoteMainWindow(e.Guid, applicationEvents, this));
                        }
                        else if (e.InterfaceType == typeof(ITextBox).FullName)
                        {
                            store.TryAdd(e.Guid, new RemoteTextBox(e.Guid, applicationEvents));
                        }
                        else if (e.InterfaceType == typeof(IComboBox).FullName)
                        {
                            store.TryAdd(e.Guid, new RemoteComboBox(e.Guid, applicationEvents));
                        }
                        else if (e.InterfaceType == typeof(IApplication).FullName)
                        {
                            application = new RemoteTreatmentApplication(e.Guid, applicationEvents, this);
                            ApplicationAvailable?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            store.TryAdd(e.Guid, e.InterfaceType);
                        }
                    }),
            };
        }

        public event EventHandler ApplicationAvailable;

        [CanBeNull] public object GetByGuid(Guid guid)
        {
            store.TryGetValue(guid, out var result);
            return result;
        }

        public void Dispose()
        {
            disposable.Dispose();
            store.Clear();
        }

        [CanBeNull]
        public IApplication GetApplication()
        {
            return application;
        }
    }
}
