namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;

    internal class MainViewStatusBarAdapter : IMainViewStatusBar
    {
        [NotNull] private readonly StatusBar item;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public MainViewStatusBarAdapter([NotNull] StatusBar item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;
            this.eventPublisher = eventPublisher;

//            foreach (UIElement child in item.Items)
//            {
//                if (child != null)
//                {
//                    var result = FieldsHelper.FindFieldInUiElementByNameOrNull<TextBlock>(child, nameof(StatusText));
//                    if (result != null)
//                        StatusText = new TextBlockAdapter(result, eventPublisher);
//                }
//            }
//
//            StatusText = new TextBlockAdapter(
//                FieldsHelper.FindFieldInUiElementByName<TextBlock>(item, nameof(StatusText)),
//                eventPublisher);

//            StatusConfigFilename = new TextBlockAdapter(
//                FieldsHelper.FindFieldInUiElementByName<TextBlock>(item, nameof(StatusConfigFilename)),
//                eventPublisher);
//
//            StatusDelayProcessCounter = new TextBlockAdapter(
//                FieldsHelper.FindFieldInUiElementByName<TextBlock>(item, nameof(StatusDelayProcessCounter)),
//                eventPublisher);

            item.Loaded += ItemOnLoaded;
            item.DataContextChanged += ItemOnDataContextChanged;
            item.SourceUpdated += ItemOnSourceUpdated;
            ((INotifyCollectionChanged)item.Items).CollectionChanged += OnCollectionChanged;
        }

        public ITextBlock StatusText { get; }

        public ITextBlock StatusConfigFilename { get; }

        public ITextBlock StatusDelayProcessCounter { get; }

        private void Items_CurrentChanged(object sender, System.EventArgs e)
        {
            eventPublisher.PublishAsync(new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(item.Items.CurrentChanged),
                Payload = e,
            });
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            item.Items.CurrentChanged += Items_CurrentChanged;

            eventPublisher.PublishAsync(new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(INotifyCollectionChanged.CollectionChanged),
                Payload = e.Action,
            });
        }

        private void ItemOnSourceUpdated(object sender, DataTransferEventArgs e)
        {
            eventPublisher.PublishAsync(new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(item.SourceUpdated),
                Payload = e.Property.Name,
            });
        }

        private void ItemOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            eventPublisher.PublishAsync(new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(item.DataContextChanged),
                Payload = e.Property.Name,
            });
        }

        private void ItemOnLoaded(object sender, RoutedEventArgs e)
        {
            eventPublisher.PublishAsync(new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(item.Loaded),
                Payload = e.Source,
            });
        }
    }
}
