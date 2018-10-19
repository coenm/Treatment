﻿namespace Treatment.UI.View
{
    using System;
    using System.Windows;

    using Treatment.UI.ViewModel;
    using Treatment.UI.ViewModel.Settings;

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
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