namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Reflection;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;

    internal class MainViewStatusBarAdapter : IMainViewStatusBar
    {
        [NotNull] private readonly StatusBar item;
        [NotNull] private readonly IEventPublisher eventPublisher;

        private ITextBlock statusText;
        private ITextBlock statusConfigFilename;
        private ITextBlock statusDelayProcessCounter;

        public MainViewStatusBarAdapter([NotNull] StatusBar item, [NotNull] IEventPublisher eventPublisher)
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
            StatusText.Dispose();
        }

        public void Initialize()
        {
            var result = FieldsHelper.FindChild<TextBlock>(item, nameof(StatusText));
            if (result != null)
            {
                StatusText = new TextBlockAdapter(result, eventPublisher);
                eventPublisher.PublishNewControl(StatusText.Guid, typeof(TextBlockAdapter), Guid);
                StatusText.Initialize();
            }
            else
            {
                foreach (var x in item.Items)
                {
                    var result1 = FieldsHelper.FindChild<TextBlock>(x as DependencyObject, nameof(StatusText));
                    if (result1 == null)
                        continue;

                    StatusText = new TextBlockAdapter(result1, eventPublisher);
                    eventPublisher.PublishNewControl(StatusText.Guid, typeof(TextBlockAdapter), Guid);
                    StatusText.Initialize();
                    break;
                }
            }

            if (StatusText == null)
                throw new System.Exception("Could not find element.");

            result = null;
            result = FieldsHelper.FindChild<TextBlock>(item, nameof(StatusConfigFilename));
            if (result != null)
            {
                StatusConfigFilename = new TextBlockAdapter(result, eventPublisher);
                eventPublisher.PublishNewControl(StatusConfigFilename.Guid, typeof(TextBlockAdapter), Guid);
                StatusConfigFilename.Initialize();
            }
            else
            {
                foreach (var x in item.Items)
                {
                    var result1 = FieldsHelper.FindChild<TextBlock>(x as DependencyObject, nameof(StatusConfigFilename));
                    if (result1 == null)
                        continue;

                    StatusConfigFilename = new TextBlockAdapter(result1, eventPublisher);
                    eventPublisher.PublishNewControl(StatusConfigFilename.Guid, typeof(TextBlockAdapter), Guid);
                    StatusConfigFilename.Initialize();
                    break;
                }
            }

            if (StatusConfigFilename == null)
                throw new System.Exception("Could not find element.");

            result = null;
            result = FieldsHelper.FindChild<TextBlock>(item, nameof(StatusDelayProcessCounter));
            if (result != null)
            {
                StatusDelayProcessCounter = new TextBlockAdapter(result, eventPublisher);
                eventPublisher.PublishNewControl(StatusDelayProcessCounter.Guid, typeof(TextBlockAdapter), Guid);
                StatusDelayProcessCounter.Initialize();
            }
            else
            {
                foreach (var x in item.Items)
                {
                    var result1 = FieldsHelper.FindChild<TextBlock>(x as DependencyObject, nameof(StatusDelayProcessCounter));
                    if (result1 == null)
                        continue;

                    StatusDelayProcessCounter = new TextBlockAdapter(result1, eventPublisher);
                    eventPublisher.PublishNewControl(StatusDelayProcessCounter.Guid, typeof(TextBlockAdapter), Guid);
                    StatusDelayProcessCounter.Initialize();
                    break;
                }
            }

            if (StatusDelayProcessCounter == null)
                throw new System.Exception("Could not find element.");

            item.Loaded += ItemOnLoaded;
            item.DataContextChanged += ItemOnDataContextChanged;
            item.SourceUpdated += ItemOnSourceUpdated;
            ((INotifyCollectionChanged)item.Items).CollectionChanged += OnCollectionChanged;
        }

        public ITextBlock StatusText
        {
            get => statusText;

            private set
            {
                eventPublisher.PublishAssignedAsync(Guid, nameof(StatusText), value.Guid);
                statusText = value;
            }
        }

        public ITextBlock StatusConfigFilename
        {
            get => statusConfigFilename;

            private set
            {
                eventPublisher.PublishAssignedAsync(Guid, nameof(StatusConfigFilename), value.Guid);
                statusConfigFilename = value;
            }
        }

        public ITextBlock StatusDelayProcessCounter
        {
            get => statusDelayProcessCounter;

            private set
            {
                eventPublisher.PublishAssignedAsync(Guid, nameof(StatusDelayProcessCounter), value.Guid);
                statusDelayProcessCounter = value;
            }
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
    }
}
