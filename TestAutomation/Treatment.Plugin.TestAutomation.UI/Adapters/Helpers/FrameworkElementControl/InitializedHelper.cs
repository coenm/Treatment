namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;

    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Window;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class InitializedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly FrameworkElement element;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public InitializedHelper([NotNull] FrameworkElement element, [NotNull] IEventPublisher eventPublisher, [NotNull] Guid guid)
        {
            Guard.NotNull(element, nameof(element));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.element = element;
            this.eventPublisher = eventPublisher;
            Guid = guid;
        }

        public Guid Guid { get; }

        public void Initialize()
        {
            element.Initialized += ElementOnInitialized;
        }

        public void Dispose()
        {
            element.Initialized -= ElementOnInitialized;
        }

        private void ElementOnInitialized(object sender, EventArgs e)
        {
            var evt = new WindowInitialized
                      {
                          Guid = Guid,
                      };

            eventPublisher.PublishAsync(evt);
        }
    }
}
