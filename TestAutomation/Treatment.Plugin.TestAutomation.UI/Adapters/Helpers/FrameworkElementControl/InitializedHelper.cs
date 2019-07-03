namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class InitializedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly FrameworkElement element;
        [NotNull] private readonly Action<Initialized> callback;

        public InitializedHelper([NotNull] FrameworkElement element, [NotNull] Action<Initialized> callback)
        {
            Guard.NotNull(element, nameof(element));
            Guard.NotNull(callback, nameof(callback));

            this.element = element;
            this.callback = callback;
        }

        public void Initialize()
        {
            element.Initialized += ElementOnInitialized;
        }

        public void Dispose()
        {
            element.Initialized -= ElementOnInitialized;
        }

        private void ElementOnInitialized(object sender, EventArgs e)
        {
            callback.Invoke(new Initialized());
        }
    }
}
