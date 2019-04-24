namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    // https://social.msdn.microsoft.com/Forums/vstudio/en-US/29ecc8ee-26ee-4331-8f97-35ff9d3e6886/how-to-access-items-in-a-datatemplate-for-wpf-listview?forum=wpf
    internal class ListViewAdapter : IUiElement, IDisposable
    {
        [NotNull] private readonly ListView item;
        [NotNull] private readonly IEventPublisher eventPublisher;
        [CanBeNull] private Point position;
        private bool itemAdded;

        public ListViewAdapter([NotNull] ListView item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));
            this.item = item;
            this.eventPublisher = eventPublisher;

            Guid = Guid.NewGuid();
        }

        public Guid Guid { get; }

        public void Dispose()
        {
        }

        public void Initialize()
        {
            item.Loaded += ItemOnLoaded;
            item.DataContextChanged += ItemOnDataContextChanged;
            item.SelectionChanged += Item_SelectionChanged;
            item.SourceUpdated += ItemOnSourceUpdated;
            item.LayoutUpdated += ItemOnLayoutUpdated;

            item.Items.CurrentChanged += Items_CurrentChanged;

            var gv = item.View as GridView;

            item.SelectionChanged += ItemOnSelectionChanged;

            if (item.Items is INotifyCollectionChanged itemsncc)
            {
                itemsncc.CollectionChanged += OnCollectionChanged;
            }

            item.SizeChanged += ItemOnSizeChanged;

            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.LocationChanged += MainWindowOnLocationChanged;
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

        private static T GetFrameworkElementByName<T>(FrameworkElement referenceElement)
            where T : FrameworkElement
        {
            FrameworkElement child = null;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(referenceElement); i++)
            {
                child = VisualTreeHelper.GetChild(referenceElement, i) as FrameworkElement;

                if (child != null && child.GetType() == typeof(T))
                    break;

                if (child == null)
                    continue;

                child = GetFrameworkElementByName<T>(child);

                if (child != null && child.GetType() == typeof(T))
                    break;
            }

            return child as T;
        }

        private void ItemOnLayoutUpdated(object sender, EventArgs e)
        {
            if (itemAdded)
            {
                itemAdded = false;

                var xxxx = item.ItemContainerGenerator.Items[0];
                var x = item.ItemContainerGenerator.ContainerFromItem(xxxx);

                var lvi = x as ListViewItem;

                //get the item's template parent
                var templateParent = GetFrameworkElementByName<ContentPresenter>(lvi);

                //get the DataTemplate that TextBlock in.

                DataTemplate dataTemplate = item.ItemTemplate;

                if (dataTemplate != null && templateParent != null)
                {
                    var btn = dataTemplate.FindName("BtnFixCsProjectFiles", templateParent) as Button;
                    var btnAdapter = new ButtonAdapter(btn, eventPublisher);
                    eventPublisher.PublishNewContol(btnAdapter.Guid, typeof(ButtonAdapter), Guid);
                    btnAdapter.Initialize();
                }

//                if (textYear != null)
//                {
//                    MessageBox.Show(String.Format("Current item's Year is:{0}", textYear.Text));
//                }
            }

            if (IsPositionUpdated())
            {
                PublishPosition();
            }
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

        private void ItemOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            eventPublisher.PublishAsync(new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(item.SelectionChanged),
                Payload = e,
            });
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

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var x = e.NewItems[0];
                var xxxx = item.ItemContainerGenerator.Items[0];
                var lvi = item.ItemContainerGenerator.ContainerFromItem(x);
                itemAdded = true;
            }

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
