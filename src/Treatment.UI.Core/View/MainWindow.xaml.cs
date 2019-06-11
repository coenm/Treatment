namespace Treatment.UI.Core.View
{
    using ViewModel;

    public partial class MainWindow
    {
        public MainWindow(IMainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
