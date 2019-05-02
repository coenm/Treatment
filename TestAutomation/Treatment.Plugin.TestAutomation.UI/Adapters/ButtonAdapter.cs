namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    public class ButtonAdapter : IButton
    {
        [NotNull] private readonly Button item;
        [NotNull] private readonly List<IInitializable> helpers;

        public ButtonAdapter([NotNull] Button item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;

            Guid = Guid.NewGuid();

            helpers = new List<IInitializable>(5)
                      {
                          new PositionChangedHelper(item, eventPublisher, Guid),
                          new SizeChangedHelper(item, eventPublisher, Guid),
                          new EnabledChangedHelper(item, eventPublisher, Guid),
                          new KeyboardFocusHelper(item, eventPublisher, Guid),
                          new FocusHelper(item, eventPublisher, Guid),
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

            // item.Click += ItemOnClick;
        }

        // private void ItemOnClick(object sender, RoutedEventArgs e)
        // {
        //     var evt = new TestAutomationEvent
        //     {
        //         Control = item.Name,
        //         EventName = nameof(item.Click),
        //         Payload = e.OriginalSource,
        //     };
        //
        //     eventPublisher.PublishAsync(evt);
        // }

        public bool IsEnabled => item.IsEnabled;

        public double Width => item.Width;

        public double Height => item.Height;
    }
}
