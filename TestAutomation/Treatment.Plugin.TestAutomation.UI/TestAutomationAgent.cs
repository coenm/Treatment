namespace Treatment.Plugin.TestAutomation.UI
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters;
    using Treatment.Plugin.TestAutomation.UI.Settings;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using TreatmentZeroMq.ContextService;
    using ZeroMQ;

    internal class TestAutomationAgent : ITestAutomationAgent
    {
        [NotNull] private readonly List<Task> workers = new List<Task>();
        private MainWindowTestAutomationView instance;
        [NotNull] private readonly ITestAutomationSettings settings;
        [NotNull] private readonly object syncLock = new object();
        [NotNull] private readonly ZContext context;
        [CanBeNull] private Task task;
        [CanBeNull] private ZSocket socket;

        private CancellationTokenSource cts;
        private MainWindowTestAutomationView view;

        public TestAutomationAgent([NotNull] IZeroMqContextService contextService, [NotNull] ITestAutomationSettings settings)
        {
            Guard.NotNull(contextService, nameof(contextService));
            Guard.NotNull(settings, nameof(settings));

            this.settings = settings;
            context = contextService.GetContext() ?? throw new NullReferenceException();
        }

        public IApplication Application { get; private set; }

        public void AddPopupView(SettingWindowAdapter view)
        {
            view.Initialize();
        }

        public void RegisterAndInitializeApplication([NotNull] IApplication application)
        {
            Guard.NotNull(application, nameof(application));
            Application = application;
            Application.Initialize();

            if (view == null)
                return;

            Application.RegisterAndInitializeMainView(view);
            view = null;
        }

        public void RegisterAndInitializeMainView(MainWindowTestAutomationView view)
        {
            if (Application != null)
            {
                Application.RegisterAndInitializeMainView(view);
            }
            else
            {
                this.view = view;
            }
        }

        public void RegisterWorker(Task worker)
        {
            workers.Add(worker);
        }
    }
}
