namespace Treatment.TestAutomation.TestRunner.Controls.Framework
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;

    public class RemoteButton : IButton, IDisposable
    {
        [NotNull] private readonly CompositeDisposable disposable;

        public RemoteButton(Guid guid, [NotNull] IApplicationEvents applicationEvents)
        {
            Guard.NotNull(applicationEvents, nameof(applicationEvents));

            var filter = applicationEvents.Events.Where(ev => ev.Guid == guid);

            disposable = new CompositeDisposable
            {
                filter
                    .Where(ev => ev is Contract.Interfaces.Events.ButtonBase.Clicked)
                    .Subscribe(ev => { Clicked?.Invoke(this, EventArgs.Empty); }),
            };
        }

        public event EventHandler Clicked;

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}
