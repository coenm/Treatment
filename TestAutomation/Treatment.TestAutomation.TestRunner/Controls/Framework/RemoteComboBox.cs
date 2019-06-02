﻿namespace Treatment.TestAutomation.TestRunner.Controls.Framework
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.ButtonBase;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    public class RemoteComboBox : IComboBox, IDisposable
    {
        [NotNull] private readonly CompositeDisposable disposable;

        public RemoteComboBox(Guid guid, [NotNull] IApplicationEvents applicationEvents)
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
                    .Where(ev => ev is IsEnabledChanged)
                    .Subscribe(ev =>
                               {
                                   IsEnabled = ((IsEnabledChanged)ev).Enabled;
                                   IsEnabledChanged?.Invoke(this, (IsEnabledChanged)ev);
                               }),

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

                filter
                    .Where(ev => ev is DropDownOpened)
                    .Subscribe(ev => { DropDownOpened?.Invoke(this, (DropDownOpened)ev); }),

                filter
                    .Where(ev => ev is DropDownClosed)
                    .Subscribe(ev => { DropDownClosed?.Invoke(this, (DropDownClosed)ev); }),

                filter
                    .Where(ev => ev is SelectionChanged)
                    .Subscribe(ev =>
                    {
                        SelectedItem = ((SelectionChanged)ev).SelectedItem;
                        SelectionChanged?.Invoke(this, (SelectionChanged)ev);
                    }),
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

        public event EventHandler<DropDownOpened> DropDownOpened;

        public event EventHandler<DropDownClosed> DropDownClosed;

        public event EventHandler<SelectionChanged> SelectionChanged;

        public bool HasFocus { get; private set; }

        public Point Position { get; private set; }

        public Size Size { get; private set; }

        public bool IsEnabled { get; private set; }

        public string SelectedItem { get; set; }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}
