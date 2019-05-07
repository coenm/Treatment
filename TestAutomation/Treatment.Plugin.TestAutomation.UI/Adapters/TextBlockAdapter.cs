namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.TextBlockControl;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class TextBlockAdapter : ITextBlock
    {
        [NotNull] private readonly TextBlock item;
        [NotNull] private readonly IEventPublisher eventPublisher;
        [NotNull] private readonly List<IInitializable> helpers;

        public TextBlockAdapter([NotNull] TextBlock item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;
            this.eventPublisher = eventPublisher;

            Guid = Guid.NewGuid();

            helpers = new List<IInitializable>(4)
                      {
                          new PositionChangedHelper(item, eventPublisher, Guid),
                          new SizeChangedHelper(item, eventPublisher, Guid),
                          new EnabledChangedHelper(item, eventPublisher, Guid),
                          new TextBlockTextValueChangedHelper(item, eventPublisher, Guid),
                      };
        }

        public Guid Guid { get; }

        public void Dispose()
        {
            helpers.ForEach(helper => helper.Dispose());
        }

        public void Initialize()
        {
            helpers.ForEach(helper => helper.Initialize());

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
