namespace Treatment.UI.Framework.SynchronizationContext
{
    using System.Threading;

    using Treatment.Helpers;

    public class UserInterfaceSynchronizationContextProvider : IUserInterfaceSynchronizationContextProvider
    {
        private SynchronizationContext uiSynchronizationContext;

        public SynchronizationContext UiSynchronizationContext
        {
            get => uiSynchronizationContext ?? SynchronizationContext.Current;
            private set => uiSynchronizationContext = value;
        }

        public void Set(SynchronizationContext synchronizationContext)
        {
            UiSynchronizationContext = Guard.NotNull(synchronizationContext, nameof(synchronizationContext));
        }
    }
}
