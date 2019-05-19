namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl
{
    using System;
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class DropDownOpenClosedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly ComboBox combobox;
        [NotNull] private readonly Action<DropDownOpened> dropDownOpenedCallback;
        [NotNull] private readonly Action<DropDownClosed> dropDownClosedCallback;

        public DropDownOpenClosedHelper(
            [NotNull] ComboBox combobox,
            [NotNull] Action<DropDownOpened> dropDownOpenedCallback,
            [NotNull] Action<DropDownClosed> dropDownClosedCallback)
        {
            Guard.NotNull(combobox, nameof(combobox));
            Guard.NotNull(dropDownOpenedCallback, nameof(dropDownOpenedCallback));
            Guard.NotNull(dropDownClosedCallback, nameof(dropDownClosedCallback));

            this.combobox = combobox;
            this.dropDownOpenedCallback = dropDownOpenedCallback;
            this.dropDownClosedCallback = dropDownClosedCallback;
        }

        public void Initialize()
        {
            combobox.DropDownOpened += ComboboxOnDropDownOpened;
            combobox.DropDownClosed += ComboboxOnDropDownClosed;
        }

        public void Dispose()
        {
            combobox.DropDownOpened -= ComboboxOnDropDownOpened;
            combobox.DropDownClosed -= ComboboxOnDropDownClosed;
        }

        private void ComboboxOnDropDownOpened(object sender, EventArgs e)
        {
            dropDownOpenedCallback.Invoke(new DropDownOpened());
        }

        private void ComboboxOnDropDownClosed(object sender, EventArgs e)
        {
            dropDownClosedCallback.Invoke(new DropDownClosed());
        }
    }
}
