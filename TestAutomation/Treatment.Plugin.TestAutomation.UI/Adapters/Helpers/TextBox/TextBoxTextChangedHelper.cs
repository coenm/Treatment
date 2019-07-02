namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.TextBox
{
    using System;
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class TextBoxTextChangedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly TextBox frameworkElement;
        [NotNull] private readonly Action<TextValueChanged> callback;

        public TextBoxTextChangedHelper([NotNull] TextBox frameworkElement, [NotNull] Action<TextValueChanged> callback)
        {
            Guard.NotNull(frameworkElement, nameof(frameworkElement));
            Guard.NotNull(callback, nameof(callback));

            this.frameworkElement = frameworkElement;
            this.callback = callback;
        }

        public void Initialize()
        {
            frameworkElement.TextChanged += FrameworkElementOnTextChanged;
        }

        public void Dispose()
        {
            frameworkElement.TextChanged -= FrameworkElementOnTextChanged;
        }

        private void FrameworkElementOnTextChanged(object sender, TextChangedEventArgs e)
        {
            callback.Invoke(new TextValueChanged
            {
                Text = frameworkElement.Text,
            });
        }
    }
}
