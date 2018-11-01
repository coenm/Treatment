namespace Treatment.UI.Framework.SynchronizationContext
{
    using System.Threading;

    using JetBrains.Annotations;

    public interface IUserInterfaceSynchronizationContextProvider
    {
        [NotNull]
        SynchronizationContext UiSynchronizationContext { get; }
    }
}
