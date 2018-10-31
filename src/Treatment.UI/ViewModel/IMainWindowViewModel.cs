﻿namespace Treatment.UI.ViewModel
{
    using System.Windows.Input;

    public interface IMainWindowViewModel
    {
        IProjectCollectionViewModel ProjectCollection { get; }

        ICommand OpenSettings { get; }

        IStatusViewModel StatusViewModel { get; }
    }

    /// <remarks>
    /// Only to be used as a DesignInstance in xaml.
    /// </remarks>>
    public abstract class DummyMainWindowViewModel : IMainWindowViewModel
    {
        public abstract ICommand OpenSettings { get; }

        public abstract IStatusViewModel StatusViewModel { get; }

        public abstract IProjectCollectionViewModel ProjectCollection { get; }
    }
}
