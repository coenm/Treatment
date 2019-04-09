namespace Treatment.Plugin.TestAutomation.UI
{
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    public class ButtonAdapter : IButton
    {
        [NotNull] private readonly Button button;

        public ButtonAdapter([NotNull] Button button)
        {
            Guard.NotNull(button, nameof(button));
            this.button = button;
        }

        public bool IsEnabled => button.IsEnabled;

        public double Width => button.Width;

        public double Height => button.Height;
    }
}
