namespace Treatment.UI.View
{
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
