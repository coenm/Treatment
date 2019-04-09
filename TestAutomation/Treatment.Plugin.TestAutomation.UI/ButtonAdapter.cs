namespace Treatment.Plugin.TestAutomation.UI
{
    using System.Windows;
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Infrastructure;
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
        }

        private void ItemOnClick(object sender, RoutedEventArgs e)
        {
            var evt = new TestAutomationEvent
            {
                Control = "name",
                EventName = nameof(item.Click),
                Payload = e.OriginalSource,
            };

            eventPublisher.PublishAsync(evt);
        }

        private void ItemOnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var evt = new TestAutomationEvent
            {
                Control = "name",
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
