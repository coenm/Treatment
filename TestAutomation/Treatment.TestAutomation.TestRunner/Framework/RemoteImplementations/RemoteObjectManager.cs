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
                        else
                        {
                            store.TryAdd(e.Guid, e.InterfaceType);
                        }
                    }),
            };
        }

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
    }
}
