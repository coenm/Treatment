﻿namespace Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations
{
    using System;
    using System.Collections.Concurrent;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using JetBrains.Annotations;
    using Treatment.TestAutomation.Contract.Interfaces.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Events;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.TestRunner.Controls.Framework;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    /// <summary>
    /// Managing/storing the creation and destruction of remote objects based on the events..
    /// </summary>
    public class RemoteObjectManager : IDisposable
    {
        [NotNull] private readonly CompositeDisposable disposable;
        [NotNull] private readonly ConcurrentDictionary<Guid, object> store;
        [CanBeNull] private ITreatmentApplication application;

        public RemoteObjectManager([NotNull] IApplicationEvents applicationEvents)
        {
            Treatment.Helpers.Guards.Guard.NotNull(applicationEvents, nameof(applicationEvents));

            store = new ConcurrentDictionary<Guid, object>();

            disposable = new CompositeDisposable
            {
                applicationEvents.Events
                    .Where(ev => ev is OnUnLoaded)
                    .Subscribe(ev =>
                    {
                        if (!store.TryRemove(ev.Guid, out var value))
                            return;

                        if (value is IDisposable disposableObject)
                            disposableObject.Dispose();
                    }),

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
                        else if (e.InterfaceType == typeof(ICheckBox).FullName)
                        {
                            store.TryAdd(e.Guid, new RemoteCheckBox(e.Guid, applicationEvents));
                        }
                        else if (e.InterfaceType == typeof(IApplication).FullName)
                        {
                            application = new ApplicationAdapter(new RemoteApplicationImplementation(e.Guid, applicationEvents, this), applicationEvents);
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

        [CanBeNull]
        public object GetByGuid(Guid guid)
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
        public ITreatmentApplication GetApplication() => application;
    }
}
