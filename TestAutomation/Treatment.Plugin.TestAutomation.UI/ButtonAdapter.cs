namespace Treatment.Plugin.TestAutomation.UI
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    public class ButtonAdapter : IButton
    {
        [NotNull] private readonly Button item;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public ButtonAdapter([NotNull] Button item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;
            this.eventPublisher = eventPublisher;

            item.IsEnabledChanged += ItemOnIsEnabledChanged;
            item.Click += ItemOnClick;
            item.FocusableChanged += ItemFocusableChanged;
            item.GotFocus += ItemOnGotFocus;
            item.GotKeyboardFocus += ItemOnGotKeyboardFocus;
            item.LostKeyboardFocus += ItemOnLostKeyboardFocus;
        }

        private void ItemOnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var evt = new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(item.LostKeyboardFocus),
                Payload = e.OriginalSource,
            };

            eventPublisher.PublishAsync(evt);
        }

        private void ItemOnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var evt = new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(item.GotKeyboardFocus),
                Payload = e.OriginalSource,
            };

            eventPublisher.PublishAsync(evt);
        }

        private void ItemOnGotFocus(object sender, RoutedEventArgs e)
        {
            var evt = new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(item.GotFocus),
                Payload = e.OriginalSource,
            };

            eventPublisher.PublishAsync(evt);
        }

        private void ItemFocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var evt = new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(item.FocusableChanged),
                Payload = e.NewValue,
            };

            eventPublisher.PublishAsync(evt);
        }

        private void ItemOnClick(object sender, RoutedEventArgs e)
        {
            var evt = new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(item.Click),
                Payload = e.OriginalSource,
            };

            eventPublisher.PublishAsync(evt);
        }

        private void ItemOnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var evt = new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(item.IsEnabledChanged),
                Payload = e.NewValue,
            };

            eventPublisher.PublishAsync(evt);
        }

        public bool IsEnabled => item.IsEnabled;

        public double Width => item.Width;

        public double Height => item.Height;
    }
}
