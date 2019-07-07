namespace Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.CheckBox
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class CheckBoxOnCheckedChangedHelper : IUiElement, IInitializable, IDisposable
    {
        [NotNull] private readonly CheckBox checkBox;
        [NotNull] private readonly Action<OnChecked> callbackOnChecked;
        [NotNull] private readonly Action<OnUnChecked> callbackOnUnChecked;

        public CheckBoxOnCheckedChangedHelper(
            [NotNull] CheckBox checkBox,
            [NotNull] Action<OnChecked> callbackOnChecked,
            [NotNull] Action<OnUnChecked> callbackOnUnChecked)
        {
            Guard.NotNull(checkBox, nameof(checkBox));
            Guard.NotNull(callbackOnChecked, nameof(callbackOnChecked));
            Guard.NotNull(callbackOnUnChecked, nameof(callbackOnUnChecked));

            this.checkBox = checkBox;
            this.callbackOnChecked = callbackOnChecked;
            this.callbackOnUnChecked = callbackOnUnChecked;
        }

        public void Initialize()
        {
            checkBox.Checked += CheckBoxOnChecked;
            checkBox.Unchecked += CheckBoxOnUnchecked;

            if (!checkBox.IsChecked.HasValue)
                return;

            if (checkBox.IsChecked.Value)
            {
                CheckBoxOnChecked(this, null);
                return;
            }

            CheckBoxOnUnchecked(this, null);
        }

        public void Dispose()
        {
            checkBox.Checked -= CheckBoxOnChecked;
            checkBox.Unchecked += CheckBoxOnUnchecked;
        }

        private void CheckBoxOnChecked(object sender, RoutedEventArgs e)
        {
            callbackOnChecked.Invoke(new OnChecked());
        }

        private void CheckBoxOnUnchecked(object sender, RoutedEventArgs e)
        {
            callbackOnUnChecked.Invoke(new OnUnChecked());
        }
    }
}
