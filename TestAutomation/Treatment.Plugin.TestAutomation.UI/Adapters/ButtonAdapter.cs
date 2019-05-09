namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.Button;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl;
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

            if (item.Name == "OpenSettingsButton")
            {
                Guid = Guid.Parse("30A6A1F1-5E47-4F45-B6AF-20A705C29A47");
            }

            helpers = new List<IInitializable>(6)
                      {
                          new PositionChangedHelper(item, eventPublisher, Guid),
                          new SizeChangedHelper(item, eventPublisher, Guid),
                          new EnabledChangedHelper(item, eventPublisher, Guid),
                          new KeyboardFocusHelper(item, eventPublisher, Guid),
                          new FocusHelper(item, eventPublisher, Guid),
                          new ButtonClickedHelper(item, eventPublisher, Guid),
                      };

            eventPublisher.PublishNewControlCreatedAsync(Guid, typeof(IButton));
        }

        public Guid Guid { get; }

        public bool IsEnabled => item.IsEnabled;

        public double Width => item.Width;

        public double Height => item.Height;

        public void Dispose()
        {
            helpers.ForEach(helper => helper.Dispose());
        }

        public void Initialize()
        {
            helpers.ForEach(helper => helper.Initialize());
        }
    }
}
