namespace Wpf.Framework.Application
{
    using System.Windows;
    using JetBrains.Annotations;

    public interface IGetActivatedWindow
    {
        [CanBeNull]
        Window Current { get; }
    }
}
