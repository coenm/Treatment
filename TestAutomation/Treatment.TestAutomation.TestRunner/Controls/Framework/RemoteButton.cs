namespace Treatment.TestAutomation.TestRunner.Controls.Framework
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.ButtonBase;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    internal class ButtonFactory : IComponentFactory
    {
        [NotNull] private readonly IApplicationEvents applicationEvents;
        [NotNull] private readonly string fullname;

        public ButtonFactory([NotNull] IApplicationEvents applicationEvents)
        {
            Guard.NotNull(applicationEvents, nameof(applicationEvents));

            fullname = typeof(IButton).FullName ?? throw new InvalidOperationException();
            this.applicationEvents = applicationEvents;
        }

        public bool CanCreate(string type) => fullname.Equals(type);

        public object Create(Guid guid) => CreateButton(guid);

        public IButton CreateButton(Guid guid) => new RemoteButton(guid, applicationEvents);
    }

    internal interface IComponentFactory
    {
        bool CanCreate(string type);

        object Create(Guid guid);
    }

    public class RemoteButton : IButton, IDisposable
    {
        [NotNull] private readonly CompositeDisposable disposable;

        public RemoteButton(Guid guid, [NotNull] IApplicationEvents applicationEvents)
        {
            Guard.NotNull(applicationEvents, nameof(applicationEvents));

            var filter = applicationEvents.Events.Where(ev => ev.Guid == guid);

            disposable = new CompositeDisposable
            {
                filter
                    .Where(ev => ev is Clicked)
                    .Subscribe(ev => { Clicked?.Invoke(this, (Clicked)ev); }),

                filter
                    .Where(ev => ev is PositionUpdated)
                    .Subscribe(ev => { PositionUpdated?.Invoke(this, (PositionUpdated)ev); }),

                filter
                    .Where(ev => ev is IsEnabledChanged)
                    .Subscribe(ev => { IsEnabledChanged?.Invoke(this, (IsEnabledChanged)ev); }),

                filter
                    .Where(ev => ev is SizeUpdated)
                    .Subscribe(ev => { SizeUpdated?.Invoke(this, (SizeUpdated)ev); }),
            };
        }

        public event EventHandler<Clicked> Clicked;

        public event EventHandler<PositionUpdated> PositionUpdated;

        public event EventHandler<IsEnabledChanged> IsEnabledChanged;

        public event EventHandler<SizeUpdated> SizeUpdated;

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}
