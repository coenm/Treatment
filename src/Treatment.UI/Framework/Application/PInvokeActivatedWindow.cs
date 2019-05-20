namespace Treatment.UI.Framework.Application
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;

    internal class PInvokeActivatedWindow : IGetActivatedWindow
    {
        public Window Current
        {
            get
            {
                try
                {
                    var active = GetActiveWindow();

                    return Application.Current.Windows.OfType<Window>()
                                      .SingleOrDefault(window => new WindowInteropHelper(window).Handle == active);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
    }
}
