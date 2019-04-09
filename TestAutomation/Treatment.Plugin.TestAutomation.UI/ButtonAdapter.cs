﻿namespace Treatment.Plugin.TestAutomation.UI
{
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    public class ButtonAdapter : IButton
    {
        [NotNull] private readonly Button item;

        public ButtonAdapter([NotNull] Button item)
        {
            Guard.NotNull(item, nameof(item));
            this.item = item;
        }

        public bool IsEnabled => item.IsEnabled;

        public double Width => item.Width;

        public double Height => item.Height;
    }
}
