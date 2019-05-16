namespace Treatment.TestAutomation.TestRunner.Controls.Framework
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Windows;

    using JetBrains.Annotations;

    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;
    using Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations;

    public class RemoteMainViewStatusBar : IMainViewStatusBar, IDisposable
    {
        [NotNull] private readonly CompositeDisposable disposable;

        public RemoteMainViewStatusBar(Guid guid, [NotNull] IApplicationEvents applicationEvents, RemoteObjectManager remoteObjectManager)
        {
            Guard.NotNull(applicationEvents, nameof(applicationEvents));

            var filter = applicationEvents.Events.Where(ev => ev.Guid == guid);

            disposable = new CompositeDisposable
                         {
                             filter
                                 .Where(ev => ev is PositionUpdated)
                                 .Subscribe(ev =>
                                            {
                                                Position = ((PositionUpdated)ev).Point;
                                                PositionUpdated?.Invoke(this, (PositionUpdated)ev);
                                            }),

                             filter
                                 .Where(ev => ev is SizeUpdated)
                                 .Subscribe(ev =>
                                            {
                                                Size = ((SizeUpdated)ev).Size;
                                                SizeUpdated?.Invoke(this, (SizeUpdated)ev);
                                            }),

                             filter
                                 .Where(ev => ev is UiElementAssigned)
                                 .Subscribe(ev =>
                                            {
                                                var e = ev as UiElementAssigned;
                                                switch (e.PropertyName)
                                                {
                                                    case nameof(StatusText):
                                                        StatusText = remoteObjectManager.GetByGuid(e.ChildElement) as ITextBlock;
                                                        break;
                                                    case nameof(StatusConfigFilename):
                                                        StatusConfigFilename = remoteObjectManager.GetByGuid(e.ChildElement) as ITextBlock;
                                                        break;
                                                    case nameof(StatusDelayProcessCounter):
                                                        StatusDelayProcessCounter = remoteObjectManager.GetByGuid(e.ChildElement) as ITextBlock;
                                                        break;
                                                }
                                            }),
                         };
        }

        public event EventHandler<PositionUpdated> PositionUpdated;

        public event EventHandler<SizeUpdated> SizeUpdated;

        public event EventHandler<Loaded> Loaded;

        public Point Position { get; private set; }

        public Size Size { get; private set; }

        public ITextBlock StatusText { get; private set; }

        public ITextBlock StatusConfigFilename { get; private set; }

        public ITextBlock StatusDelayProcessCounter { get; private set; }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}
