namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;

    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class FocusHelper : IUiElement, IDisposable
    {
        [NotNull] private readonly FrameworkElement frameworkElement;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public FocusHelper([NotNull] FrameworkElement frameworkElement, [NotNull] IEventPublisher eventPublisher, [NotNull] Guid guid)
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
            frameworkElement.FocusableChanged += FocusableChanged;
            frameworkElement.GotFocus += GotFocus;
            frameworkElement.LostFocus += LostFocus;
        }

        public void Dispose()
        {
            frameworkElement.FocusableChanged -= FocusableChanged;
            frameworkElement.GotFocus -= GotFocus;
            frameworkElement.LostFocus -= LostFocus;
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            var evt = new GotFocus
                      {
                          Guid = Guid,
                      };

            eventPublisher.PublishAsync(evt);
        }

        private void LostFocus(object sender, RoutedEventArgs e)
        {
            var evt = new LostFocus
                      {
                          Guid = Guid,
                      };

            eventPublisher.PublishAsync(evt);
        }

        private void FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var evt = new FocusableChanged
                      {
                          Guid = Guid,
                      };

            eventPublisher.PublishAsync(evt);
        }
    }
}
