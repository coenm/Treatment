namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.TextBlockControl
{
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class TextBlockTextValueChangedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly TextBlock frameworkElement;
        [NotNull] private readonly Action<TextValueChanged> callback;
        [NotNull] private readonly DependencyPropertyDescriptor dpd;

        public TextBlockTextValueChangedHelper([NotNull] TextBlock frameworkElement, [NotNull] Action<TextValueChanged> callback)
        {
            Guard.NotNull(frameworkElement, nameof(frameworkElement));
            Guard.NotNull(callback, nameof(callback));

            this.frameworkElement = frameworkElement;
            this.callback = callback;

            // https://stackoverflow.com/questions/703167/how-to-detect-a-change-in-the-text-property-of-a-textblock
            dpd = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));
        }

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
            callback.Invoke(
                new TextValueChanged
                {
                    Text = frameworkElement.Text,
                });
        }
    }
}
