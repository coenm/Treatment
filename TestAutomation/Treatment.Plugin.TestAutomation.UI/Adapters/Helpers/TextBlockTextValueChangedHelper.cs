namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers
{
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;

    using JetBrains.Annotations;

    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class TextBlockTextValueChangedHelper : IUiElement, IDisposable
    {
        [NotNull] private readonly TextBlock frameworkElement;
        [NotNull] private readonly IEventPublisher eventPublisher;
        [NotNull] private readonly DependencyPropertyDescriptor dpd;

        public TextBlockTextValueChangedHelper([NotNull] TextBlock frameworkElement, [NotNull] IEventPublisher eventPublisher, [NotNull] Guid guid)
        {
            Guard.NotNull(frameworkElement, nameof(frameworkElement));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.frameworkElement = frameworkElement;
            this.eventPublisher = eventPublisher;
            Guid = guid;

            // https://stackoverflow.com/questions/703167/how-to-detect-a-change-in-the-text-property-of-a-textblock
            dpd = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));
        }

        public Guid Guid { get; }

        public void Initialize()
        {
            dpd.AddValueChanged(frameworkElement, Handler);
        }

        public void Dispose()
        {
            dpd.RemoveValueChanged(frameworkElement, Handler);
        }

        private void Handler(object sender, EventArgs e)
        {
            eventPublisher.PublishAsync(
                new TextValueChanged
                {
                    Guid = Guid,
                    Text = frameworkElement.Text,
                });
        }
    }
}
