namespace Treatment.UI.View
{
    using System.Windows;

    using Treatment.UI.ViewModel;

    public partial class MainWindow : Window
    {
        public MainWindow(IMainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
