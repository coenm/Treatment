namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System.Windows.Controls;
    using System.Windows.Input;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class TextBlockAdapter : ITextBlock
    {
        [NotNull] private readonly TextBlock item;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public TextBlockAdapter([NotNull] TextBlock item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;
            this.eventPublisher = eventPublisher;

            item.TargetUpdated += Item_TargetUpdated;
            item.TextInput += ItemOnTextInput;
        }

        private void ItemOnTextInput(object sender, TextCompositionEventArgs e)
        {
            eventPublisher.PublishAsync(new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(item.TextInput),
                Payload = e.Text + "  " + e.ControlText,
            });
        }

        private void Item_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            eventPublisher.PublishAsync(new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(item.TargetUpdated),
                Payload = e.Property,
            });
        }

        public bool IsEnabled { get; }

        public double Width { get; }

        public double Height { get; }
    }
}
