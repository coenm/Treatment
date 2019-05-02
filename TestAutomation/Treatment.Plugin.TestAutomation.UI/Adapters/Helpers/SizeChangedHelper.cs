namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;

    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class SizeChangedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly FrameworkElement frameworkElement;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public SizeChangedHelper([NotNull] FrameworkElement frameworkElement, [NotNull] IEventPublisher eventPublisher, [NotNull] Guid guid)
        {
            Guard.NotNull(frameworkElement, nameof(frameworkElement));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.frameworkElement = frameworkElement;
            this.eventPublisher = eventPublisher;
            Guid = guid;
        }

        public Guid Guid { get; }

        public void Initialize()
        {
            frameworkElement.SizeChanged += ItemOnSizeChanged;
        }

        public void Dispose()
        {
            frameworkElement.SizeChanged -= ItemOnSizeChanged;
        }

        private void ItemOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            eventPublisher.PublishAsync(new SizeUpdated
            {
                Guid = Guid,
                Size = e.NewSize,
            });
        }
    }
}
