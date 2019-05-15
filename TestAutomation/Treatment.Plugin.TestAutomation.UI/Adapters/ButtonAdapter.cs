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
    using Treatment.TestAutomation.Contract.Interfaces.Events.ButtonBase;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    public class ButtonAdapter : ITestAutomationButton
    {
        [NotNull] private readonly Button item;
        [NotNull] private readonly List<IInitializable> helpers;
        [NotNull] private readonly ControlEventPublisher publisher;

        public ButtonAdapter([NotNull] Button item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;
            Guid = Guid.NewGuid();

            publisher = new ControlEventPublisher(this, Guid, eventPublisher);

            helpers = new List<IInitializable>
                      {
                          new PositionChangedHelper(item, c => PositionUpdated?.Invoke(this, c)),
                          new SizeChangedHelper(item, eventPublisher, Guid),
                          new EnabledChangedHelper(item, c => IsEnabledChanged?.Invoke(this, c)),
                          new KeyboardFocusHelper(item, eventPublisher, Guid),
                          new FocusHelper(item, eventPublisher, Guid),
                          new ButtonClickedHelper(item, c => Clicked?.Invoke(this, c)),
                      };

            eventPublisher.PublishNewControlCreatedAsync(Guid, typeof(IButton));
        }

        public event EventHandler<Clicked> Clicked;

        public event EventHandler<PositionUpdated> PositionUpdated;

        public event EventHandler<IsEnabledChanged> IsEnabledChanged;

        public Guid Guid { get; }

        public void Initialize()
        {
            helpers.ForEach(helper => helper.Initialize());
        }

        public void Dispose()
        {
            helpers.ForEach(helper => helper.Dispose());
            publisher.Dispose();
        }
    }
}
