namespace Treatment.UI.ViewModel
{
    using Treatment.UI.Framework.ViewModel;

    public class ProgressViewModel : ViewModelBase, IProgressViewModel
    {
        public int Min
        {
            get => Properties.Get(0);
            private set => Properties.Set(value);
        }

        public int Max
        {
            get => Properties.Get(0);
            private set => Properties.Set(value);
        }

        public bool IsIndeterminate
        {
            get => Properties.Get(false);
            private set => Properties.Set(value);
        }
    }

    public interface IProgressViewModel
    {
        int Min { get; }

        int Max { get; }

        bool IsIndeterminate { get; }
    }
}
