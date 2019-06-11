namespace Treatment.UI
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using Treatment.Helpers.Guards;
    using Treatment.UI.View;

    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using (var container = Bootstrap())
            {
                RunApplication(container);
            }
        }

        private static Container Bootstrap()
        {
            var container = new Container();
            Bootstrapper.Bootstrap(container);

            container.Verify(VerificationOption.VerifyAndDiagnose);
            return container;
        }

        private static void RunApplication([NotNull] Container container)
        {
            DebugGuard.NotNull(container, nameof(container));

            try
            {
                using (AsyncScopedLifestyle.BeginScope(container))
                {
                    var app = container.GetInstance<Application>();
                    var mainWindow = container.GetInstance<MainWindow>();

                    void MainWindowOnClosed(object sender, EventArgs e)
                    {
                        mainWindow.Closed -= MainWindowOnClosed;
                        app.Shutdown(0);
                    }

                    // not sure if this is the way to do this.
                    mainWindow.Closed += MainWindowOnClosed;

                    app.Run(mainWindow);
                }
            }

            // ReSharper disable once RedundantCatchClause
            catch
            {
                // Log the exception and exit
#if DEBUG
                throw;
#endif
            }
        }
    }
}
