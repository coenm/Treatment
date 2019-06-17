namespace Treatment.UI.Core.View
{
    using Treatment.UI.Core.ViewModel;

    public partial class MainWindow
    {
        public MainWindow(IMainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
