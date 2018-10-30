namespace Treatment.UI.View
{
    using System;
    using System.Windows;

    using Treatment.UI.Core.Configuration;
    using Treatment.UI.Framework;

    public partial class SettingsWindow : Window, IEntityEditorView<ApplicationSettings>
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public void Set(IEntityEditorViewModel<ApplicationSettings> viewModel)
        {
            DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
