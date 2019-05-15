namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Collection;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    // https://social.msdn.microsoft.com/Forums/vstudio/en-US/29ecc8ee-26ee-4331-8f97-35ff9d3e6886/how-to-access-items-in-a-datatemplate-for-wpf-listview?forum=wpf
    internal class ListViewAdapter : IListView, IDisposable
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

            helpers = new List<IInitializable>(3)
                      {
                          new PositionChangedHelper(item, c => PositionUpdated?.Invoke(this, c)),
                          new SizeChangedHelper(item, c => SizeUpdated?.Invoke(this, c)),
                          new OnLoadedHelper(item, eventPublisher, Guid),
                      };

            eventPublisher.PublishNewControlCreatedAsync(Guid, typeof(IListView));
        }

        public event EventHandler<PositionUpdated> PositionUpdated;

        public event EventHandler<SizeUpdated> SizeUpdated;

        public Guid Guid { get; }

        public void Dispose()
        {
            helpers.ForEach(helper => helper.Dispose());
        }

        public void Initialize()
        {
            helpers.ForEach(helper => helper.Initialize());

            item.DataContextChanged += ItemOnDataContextChanged;
            item.LayoutUpdated += ItemOnLayoutUpdated;
            item.SelectionChanged += ItemOnSelectionChanged;

            item.Items.CurrentChanged += Items_CurrentChanged;

            /*
            // view is a gridview but for now, we don't use this info.
            var gv = item.View as GridView;
            */

            if (item.Items is INotifyCollectionChanged notifyCollectionChanged)
            {
                notifyCollectionChanged.CollectionChanged += OnCollectionChanged;
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

                // get the item's template parent
                var templateParent = GetFrameworkElementByName<ContentPresenter>(lvi);
                var dataTemplate = templateParent?.ContentTemplate;

                if (dataTemplate != null && templateParent != null)
                {
                    if (dataTemplate.FindName("BtnFixCsProjectFiles", templateParent) is Button btn)
                    {
                        var btnAdapter = new ButtonAdapter(btn, eventPublisher);
                        btnAdapter.Initialize();
                    }
                }
            }
        }

        private void ItemOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            eventPublisher.PublishAsync(new SelectionChanged
                {
                    Guid = Guid,
                    AddedCount = e.AddedItems?.Count,
                });
        }

        private void Items_CurrentChanged(object sender, EventArgs e)
        {
            eventPublisher.PublishAsync(new CurrentChanged { Guid = Guid, });
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

            eventPublisher.PublishAsync(new CollectionChanged { Guid = Guid, Action = e.Action.ToString() });
        }

        /*
        private void ItemOnSourceUpdated(object sender, DataTransferEventArgs e)
        {
            eventPublisher.PublishAsync(new TestAutomationEvent
            {
                Control = item.Name,
                EventName = nameof(item.SourceUpdated),
                Payload = e.Property.Name,
            });
        }
        */

        private void ItemOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            eventPublisher.PublishAsync(new DataContextChanged { Guid = Guid, });
        }
    }
}
