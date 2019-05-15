﻿namespace Treatment.TestAutomation.TestRunner.Controls.Framework
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

                filter
                    .Where(ev => ev is FocusableChanged)
                    .Subscribe(ev => { FocusableChanged?.Invoke(this, (FocusableChanged)ev); }),

                filter
                    .Where(ev => ev is GotFocus)
                    .Subscribe(
                               ev =>
                               {
                                   HasFocus = true;
                                   GotFocus?.Invoke(this, (GotFocus)ev);
                               }),

                filter
                    .Where(ev => ev is LostFocus)
                    .Subscribe(
                               ev =>
                               {
                                   HasFocus = false;
                                   LostFocus?.Invoke(this, (LostFocus)ev);
                               }),

                filter
                    .Where(ev => ev is KeyboardFocusChanged)
                    .Subscribe(ev => { KeyboardFocusChanged?.Invoke(this, (KeyboardFocusChanged)ev); }),
            };
        }

        public event EventHandler<Clicked> Clicked;

        public event EventHandler<PositionUpdated> PositionUpdated;

        public event EventHandler<IsEnabledChanged> IsEnabledChanged;

        public event EventHandler<SizeUpdated> SizeUpdated;

        public event EventHandler<FocusableChanged> FocusableChanged;

        public event EventHandler<GotFocus> GotFocus;

        public event EventHandler<LostFocus> LostFocus;

        public event EventHandler<KeyboardFocusChanged> KeyboardFocusChanged;

        public bool HasFocus { get; private set; }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}
