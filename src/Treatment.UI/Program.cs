namespace Treatment.UI
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Threading;

    using JetBrains.Annotations;
    using NLog;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using Treatment.Core.Bootstrap.Plugin;
    using Treatment.Helpers.Guards;
    using Treatment.UI.Core;
    using Treatment.UI.Core.View;

    public static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomainOnReflectionOnlyAssemblyResolve;
            AppDomain.CurrentDomain.ResourceResolve += CurrentDomainOnResourceResolve;
            AppDomain.CurrentDomain.TypeResolve += CurrentDomainOnTypeResolve;

            using (var container = Bootstrap())
            {
                RunApplication(container);
            }

            AppDomain.CurrentDomain.TypeResolve -= CurrentDomainOnTypeResolve;
            AppDomain.CurrentDomain.ResourceResolve -= CurrentDomainOnResourceResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= CurrentDomainOnReflectionOnlyAssemblyResolve;
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomainOnUnhandledException;
        }

        private static Assembly CurrentDomainOnTypeResolve(object sender, ResolveEventArgs args)
        {
            Logger.Error(() => $"CurrentDomainOnTypeResolve. {args.Name}, {args.RequestingAssembly.FullName}");
            return null;
        }

        private static Assembly CurrentDomainOnResourceResolve(object sender, ResolveEventArgs args)
        {
            Logger.Error(() => $"CurrentDomainOnResourceResolve. {args.Name}, {args.RequestingAssembly.FullName}");
            return null;
        }

        private static Assembly CurrentDomainOnReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Logger.Error(() => $"CurrentDomainOnReflectionOnlyAssemblyResolve. {args.Name}, {args.RequestingAssembly.FullName}");
            return null;
        }

        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Logger.Error(() => $"CurrentDomainOnAssemblyResolve. {args.Name}, {args.RequestingAssembly.FullName}");
            return null;
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                Logger.Error(ex, () => $"Unhandled exception in app domain. {ex.Message}");
            }
            else
            {
                Logger.Error("Unhandled exception in app domain.");
            }
        }

        private static Container Bootstrap()
        {
            var container = new Container();

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Options.AllowOverridingRegistrations = true;

            UiBootstrapper.Bootstrap(container);

            RegisterPlugins(container);

            container.RegisterSingleton<DispatcherObject, App>();
            container.RegisterSingleton<Application, App>();

#if DEBUG
            if (Environment.GetEnvironmentVariable("ENABLE_TEST_AUTOMATION") == null)
                container.Verify(VerificationOption.VerifyAndDiagnose);
#endif
            return container;
        }

        private static void RegisterPlugins([NotNull] Container container)
        {
            DebugGuard.NotNull(container, nameof(container));

            var pluginAssemblies = PluginFinder.FindPluginAssemblies(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));

            foreach (var assembly in pluginAssemblies)
            {
                try
                {
                    Logger.Info($"Register packages in {assembly.FullName}");
                    container.RegisterPackages(new Assembly[1] { assembly });
                }
                catch (Exception e)
                {
                    Logger.Error(e, () => $"Could not load assembly {assembly.FullName}. {e.Message}");
                }
            }
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
            catch (Exception ex)
            {
                // Log the exception and exit
                Console.WriteLine(ex.Message);
#if DEBUG
                // throw;
#endif
            }
        }
    }
}
