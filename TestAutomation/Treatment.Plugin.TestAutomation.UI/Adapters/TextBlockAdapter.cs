namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.ComponentModel;
    using System.Windows;
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
        [CanBeNull] private Point position;

        public TextBlockAdapter([NotNull] TextBlock item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;
            this.eventPublisher = eventPublisher;

            item.TargetUpdated += Item_TargetUpdated;
            item.TextInput += ItemOnTextInput;
            item.Loaded += ItemOnLoaded;
            item.LayoutUpdated += ItemOnLayoutUpdated;

            // https://stackoverflow.com/questions/703167/how-to-detect-a-change-in-the-text-property-of-a-textblock
            var dp = DependencyPropertyDescriptor.FromProperty(
                                                               TextBlock.TextProperty,
                                                               typeof(TextBlock));
            dp.AddValueChanged(item, Handler);

            item.SizeChanged += ItemOnSizeChanged;

            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.LocationChanged += MainWindowOnLocationChanged;

            if (item.IsLoaded)
                ItemOnLoaded(item, null);
        }

        private bool IsPositionUpdated()
        {
            var pos = item.PointToScreen(new Point(0d, 0d));
            if (pos == position)
                return false;

            position = pos;
            return true;
        }

        private void MainWindowOnLocationChanged(object sender, EventArgs e)
        {
            if (!IsPositionUpdated())
                return;

            PublishPosition();
        }

        private void ItemOnLayoutUpdated(object sender, EventArgs e)
        {
            if (!IsPositionUpdated())
                return;

            PublishPosition();
        }

        private void PublishPosition()
        {
            eventPublisher.PublishAsync(new TestAutomationEvent
            {
                Control = item.Name,
                EventName = "POSITION",
                Payload = position,
            });
        }

        private void ItemOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            eventPublisher.PublishAsync(new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(item.SizeChanged),
                Payload = e.NewSize.ToString(),
            });

            if (!IsPositionUpdated())
                return;

            PublishPosition();
        }

        private void Handler(object sender, EventArgs e)
        {
            eventPublisher.PublishAsync(new TestAutomationEvent
            {
                Control = item.Name,
                EventName = "TextValueChanged",
                Payload = item.Text,
            });
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
