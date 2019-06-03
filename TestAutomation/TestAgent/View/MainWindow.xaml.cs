namespace TestAgent.View
{
    using System.Windows;

    using JetBrains.Annotations;
    using TestAgent.ViewModel;
    using Treatment.Helpers.Guards;

    public partial class MainWindow : Window
    {
        public MainWindow([NotNull] ITestAgentMainWindowViewModel viewModel)
        {
            Guard.NotNull(viewModel, nameof(viewModel));

            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
