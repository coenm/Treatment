namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Interfaces;
    using Treatment.Plugin.TestAutomation.UI.Reflection;
    using Treatment.TestAutomation.Contract.Interfaces;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;

    internal class MainViewStatusBarAdapter : ITestAutomationMainViewStatusBar, IMainViewStatusBar
    {
        [NotNull] private readonly StatusBar item;
        [NotNull] private readonly IEventPublisher eventPublisher;
        [NotNull] private readonly List<IInitializable> helpers;

        private ITestAutomationTextBlock statusText;
        private ITestAutomationTextBlock statusConfigFilename;
        private ITestAutomationTextBlock statusDelayProcessCounter;

        public MainViewStatusBarAdapter([NotNull] StatusBar item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;
            this.eventPublisher = eventPublisher;

            Guid = Guid.NewGuid();

            eventPublisher.PublishNewControlCreatedAsync(Guid, typeof(IMainViewStatusBar));

            helpers = new List<IInitializable>(3)
            {
                new PositionChangedHelper(item, c => PositionUpdated?.Invoke(this, c)),
                new SizeChangedHelper(item, c => SizeUpdated?.Invoke(this, c)),
                new OnLoadedHelper(item, c => Loaded?.Invoke(this, c)),
            };
        }

        public event EventHandler<PositionUpdated> PositionUpdated;

        public event EventHandler<SizeUpdated> SizeUpdated;

        public event EventHandler<Loaded> Loaded;

        public Guid Guid { get; }

        public ITextBlock StatusText => statusText;

        public ITextBlock StatusConfigFilename => statusConfigFilename;

        public ITextBlock StatusDelayProcessCounter => statusDelayProcessCounter;

        public void Dispose()
        {
            helpers.ForEach(helper => helper.Dispose());
            ((IDisposable)StatusText)?.Dispose();
        }

        public void Initialize()
        {
            helpers.ForEach(helper => helper.Initialize());

            var result = FieldsHelper.FindChild<TextBlock>(item, nameof(StatusText));
            if (result != null)
            {
                UpdateStatusText(new TextBlockAdapter(result, eventPublisher));
                statusText.Initialize();
            }
            else
            {
                foreach (var x in item.Items)
                {
                    var result1 = FieldsHelper.FindChild<TextBlock>(x as DependencyObject, nameof(StatusText));
                    if (result1 == null)
                        continue;

                    UpdateStatusText(new TextBlockAdapter(result1, eventPublisher));
                    statusText?.Initialize();
                    break;
                }
            }

            if (StatusText == null)
                throw new Exception("Could not find element.");

            result = FieldsHelper.FindChild<TextBlock>(item, nameof(StatusConfigFilename));
            if (result != null)
            {
                UpdateStatusConfigFilename(new TextBlockAdapter(result, eventPublisher));
                statusConfigFilename?.Initialize();
            }
            else
            {
                foreach (var x in item.Items)
                {
                    var result1 = FieldsHelper.FindChild<TextBlock>(x as DependencyObject, nameof(StatusConfigFilename));
                    if (result1 == null)
                        continue;

                    UpdateStatusConfigFilename(new TextBlockAdapter(result1, eventPublisher));
                    statusConfigFilename?.Initialize();
                    break;
                }
            }

            if (StatusConfigFilename == null)
                throw new Exception("Could not find element.");

            result = FieldsHelper.FindChild<TextBlock>(item, nameof(StatusDelayProcessCounter));
            if (result != null)
            {
                UpdateStatusDelayProcessCounter(new TextBlockAdapter(result, eventPublisher));
                statusDelayProcessCounter?.Initialize();
            }
            else
            {
                foreach (var x in item.Items)
                {
                    var result1 = FieldsHelper.FindChild<TextBlock>(x as DependencyObject, nameof(StatusDelayProcessCounter));
                    if (result1 == null)
                        continue;

                    UpdateStatusDelayProcessCounter(new TextBlockAdapter(result1, eventPublisher));
                    statusDelayProcessCounter?.Initialize();
                    break;
                }
            }

            if (StatusDelayProcessCounter == null)
                throw new Exception("Could not find element.");

            item.DataContextChanged += ItemOnDataContextChanged;
            item.SourceUpdated += ItemOnSourceUpdated;

            ((INotifyCollectionChanged)item.Items).CollectionChanged += OnCollectionChanged;
        }

        private void UpdateStatusDelayProcessCounter(ITestAutomationTextBlock value)
        {
            statusDelayProcessCounter = value;

            if (statusDelayProcessCounter != null)
                eventPublisher.PublishAssignedAsync(Guid, nameof(StatusDelayProcessCounter), statusDelayProcessCounter.Guid);
            else
                eventPublisher.PublishClearedAsync(Guid, nameof(StatusDelayProcessCounter));
        }

        private void UpdateStatusText(ITestAutomationTextBlock value)
        {
            statusText = value;

            if (statusText != null)
                eventPublisher.PublishAssignedAsync(Guid, nameof(StatusText), statusText.Guid);
            else
                eventPublisher.PublishClearedAsync(Guid, nameof(StatusText));
        }

        private void UpdateStatusConfigFilename(ITestAutomationTextBlock value)
        {
            statusConfigFilename = value;

            if (statusConfigFilename != null)
                eventPublisher.PublishAssignedAsync(Guid, nameof(StatusConfigFilename), statusConfigFilename.Guid);
            else
                eventPublisher.PublishClearedAsync(Guid, nameof(StatusConfigFilename));
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
    }
}
