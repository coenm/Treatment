namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;
    using Events.Element;

    public interface IElementLoadedUnLoaded
    {
        event EventHandler<OnLoaded> OnLoaded;

        event EventHandler<OnUnLoaded> OnUnLoaded;
    }
}
