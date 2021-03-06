﻿namespace TestAgent.View
{
    using System.Windows;

    using TestAgent.Model.Configuration;
    using Treatment.Helpers.Guards;
    using Wpf.Framework.EntityEditor.View;
    using Wpf.Framework.EntityEditor.ViewModel;

    public partial class SettingsWindow : IEntityEditorView<TestAgentApplicationSettings>
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public void Set(IEntityEditorViewModel<TestAgentApplicationSettings> viewModel)
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
