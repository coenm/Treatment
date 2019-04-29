namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class TextBlockAdapter : ITextBlock
    {
        [NotNull] private readonly TextBlock item;
        [NotNull] private readonly IEventPublisher eventPublisher;
        [NotNull] private readonly PositionChangedHelper helper1;
        [NotNull] private readonly SizeChangedHelper helper2;
        [NotNull] private readonly EnabledChangedHelper helper3;
        [NotNull] private readonly TextBlockTextValueChangedHelper helper4;

        public TextBlockAdapter([NotNull] TextBlock item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;
            this.eventPublisher = eventPublisher;

            Guid = Guid.NewGuid();

            helper1 = new PositionChangedHelper(item, eventPublisher, Guid);
            helper2 = new SizeChangedHelper(item, eventPublisher, Guid);
            helper3 = new EnabledChangedHelper(item, eventPublisher, Guid);
            helper4 = new TextBlockTextValueChangedHelper(item, eventPublisher, Guid);
        }

        public Guid Guid { get; }

        public void Dispose()
        {
            helper4.Dispose();
            helper3.Dispose();
            helper2.Dispose();
            helper1.Dispose();
        }

        public void Initialize()
        {
            helper1.Initialize();
            helper2.Initialize();
            helper3.Initialize();
            helper4.Initialize();

            item.TargetUpdated += Item_TargetUpdated;
            item.TextInput += ItemOnTextInput;
            item.Loaded += ItemOnLoaded;

            if (item.IsLoaded)
                ItemOnLoaded(item, null);
        }

        private void ItemOnLoaded(object sender, RoutedEventArgs e)
        {
            eventPublisher.PublishAsync(new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(item.Loaded),
                Payload = null,
            });
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
