namespace Treatment.UI.ViewModel
{
    using NLog;
    using Treatment.Contract;
    using Wpf.Framework.ViewModel;

    public class ProgressViewModel : ViewModelBase, IProgressViewModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public int Value
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

        public void Update(ProgressData progressData)
        {
            Logger.Debug(() => $"Value: {progressData.Position.CurrentValue}; " +
                               $"Max:{progressData.Position.Max}; " +
                               $"IsIndeterminate:{Value == 0 && Max == 0}");
            Value = progressData.Position.CurrentValue;
            Max = progressData.Position.Max;
            IsIndeterminate = Value < 0 || Max <= 0;
        }
    }
}
