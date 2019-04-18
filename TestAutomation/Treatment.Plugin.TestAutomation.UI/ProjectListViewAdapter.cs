namespace Treatment.Plugin.TestAutomation.UI
{
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Infrastructure;
    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;
    using Treatment.UI.UserControls;

    internal class ProjectListViewAdapter : IProjectListView
    {
        [NotNull] private readonly ProjectListView item;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public ProjectListViewAdapter([NotNull] ProjectListView item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;
            this.eventPublisher = eventPublisher;

            var fields = item.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField).ToList();

            var fields1 = fields.Where(x => x.Name == nameof(Listview)).ToList();
            var fields2 = fields1.Where(x => x.FieldType == typeof(ListView)).ToList();

            var sod = fields2.SingleOrDefault();
            if (sod != null)
                Listview = new ListViewAdapter((ListView)sod.GetValue(item), eventPublisher);
            else
                throw new CouldNotFindFieldException(nameof(Listview));
        }

        public bool IsEnabled => item.IsEnabled;

        public double Width => item.Width;

        public double Height => item.Height;

        public ListViewAdapter Listview { get; }
    }

    internal class ListViewAdapter
    {
        [NotNull] private readonly ListView item;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public ListViewAdapter([NotNull] ListView item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));
            this.item = item;
            this.eventPublisher = eventPublisher;

            item.Loaded += ItemOnLoaded;
            item.DataContextChanged += ItemOnDataContextChanged;
            item.SelectionChanged += Item_SelectionChanged;
            item.SourceUpdated += ItemOnSourceUpdated;
            ((INotifyCollectionChanged)item.Items).CollectionChanged += OnCollectionChanged;
        }

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

        private void Item_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            eventPublisher.PublishAsync(new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(item.SelectionChanged),
                Payload = e.AddedItems?.Count,
            });
        }
    }
}
