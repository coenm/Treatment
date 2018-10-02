namespace Treatment.UI.ViewModel
{
    public class MainWindowViewModel : IMainWindowViewModel
    {
        public MainWindowViewModel()
        {
            Name = "test";
        }

        public string Name { get; set; }
    }
}