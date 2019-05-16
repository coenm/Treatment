namespace Treatment.TestAutomation.TestRunner.Controls.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;
    using Treatment.TestAutomation.Contract.Interfaces.Events;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations;

    internal class SingleClassObjectManager : IDisposable
    {
        [NotNull] private readonly RemoteObjectManager remoteObjectManager;
        [NotNull] private readonly CompositeDisposable disposable;
        [NotNull] private readonly Dictionary<string, Guid> propertyGuids;

        public SingleClassObjectManager([NotNull] RemoteObjectManager remoteObjectManager, [NotNull] IObservable<IEvent> observer)
        {
            this.remoteObjectManager = remoteObjectManager;
            propertyGuids = new Dictionary<string, Guid>();

            disposable = new CompositeDisposable
            {
                observer
                    .Where(ev => ev is UiElementAssigned)
                    .Subscribe(
                        ev =>
                        {
                            var e = ev as UiElementAssigned;

                            if (!string.IsNullOrWhiteSpace(e?.PropertyName))
                                propertyGuids[e.PropertyName] = e.ChildElement;
                        }),
            };
        }

        public T GetObject<T>([CallerMemberName] string key = "")
            where T : class
        {
            if (!propertyGuids.TryGetValue(key, out var guid))
                return null;

            return remoteObjectManager.GetByGuid(guid) as T;
        }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}
