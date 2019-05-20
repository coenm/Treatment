namespace Treatment.UI.Framework.Application
{
    using System.Windows;

    using JetBrains.Annotations;

    internal interface IGetActivatedWindow
    {
        [CanBeNull]
        Window Current { get; }
    }
}
