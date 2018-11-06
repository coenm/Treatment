namespace Treatment.UI.Framework.SynchronizationContext
{
    using System.Threading;
    using System.Windows.Threading;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;

    public class UserInterfaceSynchronizationContextProvider : IUserInterfaceSynchronizationContextProvider
    {
        // ReSharper disable once NotNullMemberIsNotInitialized; Justification: Property is using the dispatcher.
        public UserInterfaceSynchronizationContextProvider([NotNull] DispatcherObject uiDispatcherObject)
        {
            Guard.NotNull(uiDispatcherObject, nameof(uiDispatcherObject));
            uiDispatcherObject.Dispatcher.Invoke(() => UiSynchronizationContext = SynchronizationContext.Current);
        }

        public SynchronizationContext UiSynchronizationContext { get; private set; }
    }
}
