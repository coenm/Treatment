namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class OnLoadedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly FrameworkElement frameworkElement;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public OnLoadedHelper([NotNull] FrameworkElement frameworkElement, [NotNull] IEventPublisher eventPublisher, [NotNull] Guid guid)
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
            frameworkElement.Loaded += Loaded;
        }

        public void Dispose()
        {
            frameworkElement.Loaded -= Loaded;
        }

        private void Loaded(object sender, RoutedEventArgs e)
        {
            var evt = new Loaded
            {
                Guid = Guid,
            };

            eventPublisher.PublishAsync(evt);
        }
    }
}
