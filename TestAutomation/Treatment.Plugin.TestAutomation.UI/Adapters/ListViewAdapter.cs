namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    // https://social.msdn.microsoft.com/Forums/vstudio/en-US/29ecc8ee-26ee-4331-8f97-35ff9d3e6886/how-to-access-items-in-a-datatemplate-for-wpf-listview?forum=wpf
    internal class ListViewAdapter : IUiElement, IDisposable
    {
        [NotNull] private readonly ListView item;
        [NotNull] private readonly IEventPublisher eventPublisher;
        [NotNull] private readonly List<IInitializable> helpers;
        private bool itemAdded;

        public ListViewAdapter([NotNull] ListView item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;
            this.eventPublisher = eventPublisher;

            Guid = Guid.NewGuid();

            helpers = new List<IInitializable>(2)
                      {
                          new PositionChangedHelper(item, eventPublisher, Guid),
                          new SizeChangedHelper(item, eventPublisher, Guid),
                      };
        }

        public Guid Guid { get; }

        public void Dispose()
        {
            helpers.ForEach(item => item.Dispose());
        }

        public void Initialize()
        {
            helpers.ForEach(item => item.Initialize());

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
                    eventPublisher.PublishNewControl(btnAdapter.Guid, typeof(ButtonAdapter), Guid);
                    btnAdapter.Initialize();
                }

//                if (textYear != null)
//                {
//                    MessageBox.Show(String.Format("Current item's Year is:{0}", textYear.Text));
//                }
            }
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
