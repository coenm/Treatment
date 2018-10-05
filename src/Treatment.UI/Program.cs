using System;

using SimpleInjector;

namespace Treatment.UI
{
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

    using SimpleInjector.Lifestyles;

    using Treatment.Core;
    using Treatment.UI.ViewModel;

    static class Program
    {
        [STAThread]
        static void Main()
        {
            var container = Bootstrap();

            RunApplication(container);
        }

        private static Container Bootstrap()
        {
            // Create the container as usual.
            var container = new Container();

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Options.AllowOverridingRegistrations = true;

            CoreBootstrap.Bootstrap(container);

            // Register your windows and view models:
            container.Register<View.MainWindow>();
            container.Register<IMainWindowViewModel, MainWindowViewModel>();

            RegisterPlugins(container);

            RegisterDebug(container);

            container.Verify();

            return container;
        }

        private static void RegisterDebug([NotNull] Container container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            DelayCommandExecution.Register(container);
        }

        private static void RegisterPlugins([NotNull] Container container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            var pluginDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);

            var pluginAssemblies = new DirectoryInfo(pluginDirectory)
                                   .GetFiles()
                                   .Where(file =>
                                              file.Name.StartsWith("Treatment.Plugin.")
                                              &&
                                              file.Extension.ToLower() == ".dll")
                                   .Select(file => Assembly.Load(AssemblyName.GetAssemblyName(file.FullName)));

            container.RegisterPackages(pluginAssemblies);
        }

        private static void RunApplication([NotNull] Container container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            try
            {
                using (AsyncScopedLifestyle.BeginScope(container))
                {
                    var app = new App();
                    var mainWindow = container.GetInstance<View.MainWindow>();
                    app.Run(mainWindow);
                }
            }
            catch (Exception e)
            {
                throw e;
                //Log the exception and exit
            }
        }
    }
}