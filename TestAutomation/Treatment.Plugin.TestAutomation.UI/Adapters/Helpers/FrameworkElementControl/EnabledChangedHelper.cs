namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class EnabledChangedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly FrameworkElement frameworkElement;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public EnabledChangedHelper([NotNull] FrameworkElement frameworkElement, [NotNull] IEventPublisher eventPublisher, [NotNull] Guid guid)
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
            frameworkElement.IsEnabledChanged += IsEnabledChanged;
        }

        public void Dispose()
        {
            frameworkElement.IsEnabledChanged -= IsEnabledChanged;
        }

        private void IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            eventPublisher.PublishAsync(new IsEnabledChanged
            {
                Guid = Guid,
                Enabled = (bool)e.NewValue,
            });
        }
    }
}
