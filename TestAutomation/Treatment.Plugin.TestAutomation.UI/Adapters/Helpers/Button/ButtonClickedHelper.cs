namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.Button
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.ButtonBase;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class ButtonClickedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly ButtonBase button;
        [NotNull] private readonly Action<Clicked> callback;

        public ButtonClickedHelper([NotNull] ButtonBase button, [NotNull] Action<Clicked> callback)
        {
            Guard.NotNull(button, nameof(button));
            Guard.NotNull(callback, nameof(callback));

            this.button = button;
            this.callback = callback;
        }

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
            callback.Invoke(new Clicked());
        }
    }
}
