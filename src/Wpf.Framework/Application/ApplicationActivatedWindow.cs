namespace Wpf.Framework.Application
{
    using System;
    using System.Linq;
    using System.Windows;

    public class ApplicationActivatedWindow : IGetActivatedWindow
    {
        public Window Current
        {
            // https://stackoverflow.com/questions/2038879/refer-to-active-window-in-wpf
            get
            {
                try
                {
                    return Application.Current.Windows.OfType<Window>()
                                      .SingleOrDefault(x => x.IsActive);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
        }
    }
}
