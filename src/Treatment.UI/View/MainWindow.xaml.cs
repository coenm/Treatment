namespace Treatment.UI.View
{
    using System.Windows;

    using Treatment.UI.ViewModel;

    public partial class MainWindow
    {
        public MainWindow(IMainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
