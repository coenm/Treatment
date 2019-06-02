namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class FocusHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly System.Windows.FrameworkElement frameworkElement;
        [NotNull] private readonly Action<FocusableChanged> focusableChangedCallback;
        [NotNull] private readonly Action<GotFocus> gotFocusCallback;
        [NotNull] private readonly Action<LostFocus> lostFocusCallback;

        public FocusHelper(
            [NotNull] System.Windows.FrameworkElement frameworkElement,
            [NotNull] Action<FocusableChanged> focusableChangedCallback,
            [NotNull] Action<GotFocus> gotFocusCallback,
            [NotNull] Action<LostFocus> lostFocusCallback)
        {
            Guard.NotNull(frameworkElement, nameof(frameworkElement));
            Guard.NotNull(focusableChangedCallback, nameof(focusableChangedCallback));
            Guard.NotNull(gotFocusCallback, nameof(gotFocusCallback));
            Guard.NotNull(lostFocusCallback, nameof(lostFocusCallback));

            this.frameworkElement = frameworkElement;
            this.focusableChangedCallback = focusableChangedCallback;
            this.gotFocusCallback = gotFocusCallback;
            this.lostFocusCallback = lostFocusCallback;
        }

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
            gotFocusCallback.Invoke(new GotFocus());
        }

        private void LostFocus(object sender, RoutedEventArgs e)
        {
            lostFocusCallback.Invoke(new LostFocus());
        }

        private void FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            focusableChangedCallback.Invoke(new FocusableChanged());
        }
    }
}
