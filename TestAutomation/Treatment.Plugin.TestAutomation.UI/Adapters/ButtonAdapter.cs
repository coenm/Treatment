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
    using Treatment.Plugin.TestAutomation.UI.Interfaces;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    public class ButtonAdapter : ITestAutomationButton
    {
        [NotNull] private readonly Button item;
        [NotNull] private readonly List<IInitializable> helpers;

        public ButtonAdapter([NotNull] Button item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;

            Guid = Guid.NewGuid();

            helpers = new List<IInitializable>
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

        public void Initialize()
        {
            helpers.ForEach(helper => helper.Initialize());
        }

        public void Dispose()
        {
            helpers.ForEach(helper => helper.Dispose());
        }
    }
}
