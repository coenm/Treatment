﻿namespace Treatment.UI.Framework
{
    using System.Windows.Input;

    /// <summary>
    /// Expose a command to initialize the ViewModel.
    /// </summary>
    public interface IInitializableViewModel
    {
        ICommand Initialize { get; }
    }
}
