namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    public class ButtonAdapter : IButton
    {
        [NotNull] private readonly Button item;
        [NotNull] private readonly IEventPublisher eventPublisher;
        [NotNull] private readonly PositionChangedHelper helper1;
        [NotNull] private readonly SizeChangedHelper helper2;
        [NotNull] private readonly EnabledChangedHelper helper3;
        [NotNull] private readonly KeyboardFocusHelper helper4;
        [NotNull] private readonly FocusHelper helper5;

        public ButtonAdapter([NotNull] Button item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;
            this.eventPublisher = eventPublisher;

            Guid = Guid.NewGuid();

            helper1 = new PositionChangedHelper(item, eventPublisher, Guid);
            helper2 = new SizeChangedHelper(item, eventPublisher, Guid);
            helper3 = new EnabledChangedHelper(item, eventPublisher, Guid);
            helper4 = new KeyboardFocusHelper(item, eventPublisher, Guid);
            helper5 = new FocusHelper(item, eventPublisher, Guid);
        }

        public Guid Guid { get; }

        public void Dispose()
        {
            helper5.Dispose();
            helper4.Dispose();
            helper3.Dispose();
            helper2.Dispose();
            helper1.Dispose();
        }

        public void Initialize()
        {
            helper1.Initialize();
            helper2.Initialize();
            helper3.Initialize();
            helper4.Initialize();
            helper5.Initialize();

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
