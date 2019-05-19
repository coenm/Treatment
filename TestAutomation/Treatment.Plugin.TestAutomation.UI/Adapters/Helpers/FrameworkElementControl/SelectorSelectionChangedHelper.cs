namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class SelectorSelectionChangedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly Selector selector;
        [NotNull] private readonly Action<SelectionChanged> selectionChangedCallback;

        public SelectorSelectionChangedHelper(
            [NotNull] Selector selector,
            [NotNull] Action<SelectionChanged> selectionChangedCallback)
        {
            Guard.NotNull(selector, nameof(selector));
            Guard.NotNull(selectionChangedCallback, nameof(selectionChangedCallback));

            this.selector = selector;
            this.selectionChangedCallback = selectionChangedCallback;
        }

        public void Initialize()
        {
            selector.SelectionChanged += SelectorOnSelectionChanged;
        }

        public void Dispose()
        {
            selector.SelectionChanged -= SelectorOnSelectionChanged;
        }

        private void SelectorOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // this is not a complete implementation.
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                var first = e.AddedItems[0];
                selectionChangedCallback.Invoke(
                    new SelectionChanged
                    {
                        SelectedItem = first.ToString(),
                    });
            }
            else
            {
                selectionChangedCallback.Invoke(new SelectionChanged());
            }
        }
    }
}
