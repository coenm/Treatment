namespace Treatment.UI.Core.View
{
    using System.Windows;
    using Core.Configuration;
    using Helpers.Guards;
    using Wpf.Framework.EntityEditor.View;
    using Wpf.Framework.EntityEditor.ViewModel;

    public partial class SettingsWindow : IEntityEditorView<ApplicationSettings>
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public void Set(IEntityEditorViewModel<ApplicationSettings> viewModel)
        {
            Guard.NotNull(viewModel, nameof(viewModel));
            DataContext = viewModel;
        }

        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
