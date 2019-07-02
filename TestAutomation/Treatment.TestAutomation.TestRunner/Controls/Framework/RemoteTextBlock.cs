namespace Treatment.TestAutomation.TestRunner.Controls.Framework
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Windows;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.TestRunner.Controls.Interfaces;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    public class RemoteTextBlock : ITestRunnerControlTextBlock, IDisposable
    {
        [NotNull] private readonly CompositeDisposable disposable;

        public RemoteTextBlock(Guid guid, [NotNull] IApplicationEvents applicationEvents)
        {
            Guard.NotNull(applicationEvents, nameof(applicationEvents));

            var filter = applicationEvents.Events.Where(ev => ev.Guid == guid);

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
                                 .Where(ev => ev is IsEnabledChanged)
                                 .Subscribe(ev =>
                                    {
                                        IsEnabled = ((IsEnabledChanged)ev).Enabled;
                                        IsEnabledChanged?.Invoke(this, (IsEnabledChanged)ev);
                                    }),

                             filter
                                 .Where(ev => ev is TextValueChanged)
                                 .Subscribe(ev =>
                                    {
                                        Value = ((TextValueChanged)ev).Text;
                                        TextValueChanged?.Invoke(this, (TextValueChanged)ev);
                                    }),
                         };
        }

        public event EventHandler<PositionUpdated> PositionUpdated;

        public event EventHandler<SizeUpdated> SizeUpdated;

        public event EventHandler<IsEnabledChanged> IsEnabledChanged;

        public event EventHandler<TextValueChanged> TextValueChanged;

        public string Value { get; private set; }

        public bool HasFocus => throw new NotImplementedException();

        public Point Position { get; private set; }

        public Size Size { get; private set; }

        public bool IsEnabled { get; private set; }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}
