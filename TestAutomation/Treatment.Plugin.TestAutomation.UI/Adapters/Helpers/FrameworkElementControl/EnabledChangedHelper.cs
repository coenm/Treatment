namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class EnabledChangedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly FrameworkElement frameworkElement;
        [NotNull] private readonly Action<IsEnabledChanged> callback;

        public EnabledChangedHelper([NotNull] FrameworkElement frameworkElement, [NotNull] Action<IsEnabledChanged> callback)
        {
            Guard.NotNull(frameworkElement, nameof(frameworkElement));
            Guard.NotNull(callback, nameof(callback));

            this.frameworkElement = frameworkElement;
            this.callback = callback;
        }

        public void Initialize()
        {
            frameworkElement.IsEnabledChanged += IsEnabledChanged;

            callback.Invoke(new IsEnabledChanged
            {
                Enabled = frameworkElement.IsEnabled,
            });
        }

        public void Dispose()
        {
            frameworkElement.IsEnabledChanged -= IsEnabledChanged;
        }

        private void IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            callback.Invoke(new IsEnabledChanged
            {
                Enabled = (bool)e.NewValue,
            });
        }
    }
}
