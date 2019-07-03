namespace Treatment.TestAutomation.TestRunner.Controls.Framework
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Window;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.TestRunner.Controls.Interfaces.TreatmentControls;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;
    using Treatment.TestAutomation.TestRunner.Framework.RemoteImplementations;

    public class RemoteSettingWindow : ITestRunnerOwnControlSettingWindow, IDisposable
    {
        [NotNull] private readonly CompositeDisposable disposable;
        [NotNull] private readonly SingleClassObjectManager propertyManager;

        public RemoteSettingWindow(
            Guid guid,
            [NotNull] IApplicationEvents applicationEvents,
            [NotNull] RemoteObjectManager remoteObjectManager)
        {
            Guard.NotNull(applicationEvents, nameof(applicationEvents));
            Guard.NotNull(remoteObjectManager, nameof(remoteObjectManager));

            var filter = applicationEvents.Events.Where(ev => ev.Guid == guid);

            propertyManager = new SingleClassObjectManager(remoteObjectManager, filter);

            disposable = new CompositeDisposable
                         {
                             filter
                                 .Where(ev => ev is PositionUpdated)
                                 .Subscribe(ev =>
                                            {
                                                Position = ((PositionUpdated)ev).Point;
                                                PositionUpdated?.Invoke(this, (PositionUpdated)ev);
                                            }),

                             filter
                                 .Where(ev => ev is SizeUpdated)
                                 .Subscribe(ev =>
                                            {
                                                Size = ((SizeUpdated)ev).Size;
                                                SizeUpdated?.Invoke(this, (SizeUpdated)ev);
                                            }),

                             filter
                                 .Where(ev => ev is GotFocus)
                                 .Subscribe(
                                     ev =>
                                     {
                                         HasFocus = true;
                                         GotFocus?.Invoke(this, (GotFocus)ev);
                                     }),

                             filter
                                 .Where(ev => ev is LostFocus)
                                 .Subscribe(
                                     ev =>
                                     {
                                         HasFocus = false;
                                         LostFocus?.Invoke(this, (LostFocus)ev);
                                     }),

                             filter
                                 .Where(ev => ev is OnLoaded)
                                 .Subscribe(ev => { OnLoaded?.Invoke(this, (OnLoaded)ev); }),

                             filter
                                 .Where(ev => ev is OnUnLoaded)
                                 .Subscribe(ev => { OnUnLoaded?.Invoke(this, (OnUnLoaded)ev); }),
                         };
        }

        public event EventHandler<Initialized> Initialized;

        public event EventHandler<WindowClosing> WindowClosing;

        public event EventHandler<WindowClosed> WindowClosed;

        public event EventHandler<WindowActivated> WindowActivated;

        public event EventHandler<WindowDeactivated> WindowDeactivated;

        public event EventHandler<PositionUpdated> PositionUpdated;

        public event EventHandler<IsEnabledChanged> IsEnabledChanged;

        public event EventHandler<SizeUpdated> SizeUpdated;

        public event EventHandler<FocusableChanged> FocusableChanged;

        public event EventHandler<GotFocus> GotFocus;

        public event EventHandler<LostFocus> LostFocus;

        public event EventHandler<OnLoaded> OnLoaded;

        public event EventHandler<OnUnLoaded> OnUnLoaded;

        public IButton BrowseRootDirectory => propertyManager.GetObject<IButton>();

        public ITextBox RootDirectory => propertyManager.GetObject<ITextBox>();

        public IComboBox ComboSearchProvider => propertyManager.GetObject<IComboBox>();

        public IComboBox ComboVersionControlProvider => propertyManager.GetObject<IComboBox>();

        public ICheckBox DelayExecution => propertyManager.GetObject<ICheckBox>();

        public ITextBox DelayExecutionMinValue => propertyManager.GetObject<ITextBox>();

        public ITextBox DelayExecutionMaxValue => propertyManager.GetObject<ITextBox>();

        public IButton OkButton => propertyManager.GetObject<IButton>();

        public IButton CancelButton => propertyManager.GetObject<IButton>();

        public Point Position { get; private set; }

        public Size Size { get; private set; }

        public bool HasFocus { get; private set; }

        public void Dispose()
        {
            disposable.Dispose();
            propertyManager.Dispose();
        }
    }
}
