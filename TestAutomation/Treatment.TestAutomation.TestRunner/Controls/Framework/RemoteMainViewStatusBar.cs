namespace Treatment.TestAutomation.TestRunner.Controls.Framework
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;
    using Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations;

    public class RemoteMainViewStatusBar : IMainViewStatusBar, IDisposable
    {
        [NotNull] private readonly CompositeDisposable disposable;
        [NotNull] private readonly SingleClassObjectManager propertyManager;

        public RemoteMainViewStatusBar(
            Guid guid,
            [NotNull] IApplicationEvents applicationEvents,
            [NotNull] RemoteObjectManager remoteObjectManager)
        {
            Guard.NotNull(applicationEvents, nameof(applicationEvents));
            Guard.NotNull(remoteObjectManager, nameof(remoteObjectManager));

            var filter = applicationEvents.Events.Where(ev => ev.Guid == guid);

            propertyManager = new SingleClassObjectManager(remoteObjectManager, filter);

            disposable = new CompositeDisposable
                         {
                             filter
                                 .Where(ev => ev is PositionUpdated)
                                 .Subscribe(
                                 ev =>
                                        {
                                            Position = ((PositionUpdated)ev).Point;
                                            PositionUpdated?.Invoke(this, (PositionUpdated)ev);
                                        }),

                             filter
                                 .Where(ev => ev is SizeUpdated)
                                 .Subscribe(
                                     ev =>
                                            {
                                                Size = ((SizeUpdated)ev).Size;
                                                SizeUpdated?.Invoke(this, (SizeUpdated)ev);
                                            }),
                         };
        }

        public event EventHandler<PositionUpdated> PositionUpdated;

        public event EventHandler<SizeUpdated> SizeUpdated;

        public event EventHandler<Loaded> Loaded;

        public Point Position { get; private set; }

        public Size Size { get; private set; }

        public ITextBlock StatusText => propertyManager.GetObject<ITextBlock>();

        public ITextBlock StatusConfigFilename => propertyManager.GetObject<ITextBlock>();

        public ITextBlock StatusDelayProcessCounter => propertyManager.GetObject<ITextBlock>();

        public void Dispose()
        {
            disposable.Dispose();
            propertyManager.Dispose();
        }
    }
}
