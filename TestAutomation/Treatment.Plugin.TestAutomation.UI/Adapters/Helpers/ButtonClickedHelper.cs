namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events.ButtonBase;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class ButtonClickedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly ButtonBase button;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public ButtonClickedHelper([NotNull] ButtonBase button, [NotNull] IEventPublisher eventPublisher, [NotNull] Guid guid)
        {
            Guard.NotNull(button, nameof(button));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.button = button;
            this.eventPublisher = eventPublisher;
            Guid = guid;
        }

        public Guid Guid { get; }

        public void Initialize()
        {
            button.Click += ItemOnClick;
        }

        public void Dispose()
        {
            button.Click -= ItemOnClick;
        }

        private void ItemOnClick(object sender, RoutedEventArgs e)
        {
            var evt = new Clicked
                      {
                          Guid = Guid,
                      };

            eventPublisher.PublishAsync(evt);
        }
    }
}
