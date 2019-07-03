namespace Treatment.TestAutomation.Contract.Interfaces.Framework.SingleEventInterface
{
    using System;

    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;

    public interface IElementLoadedUnLoaded
    {
        event EventHandler<OnLoaded> OnLoaded;

        event EventHandler<OnUnLoaded> OnUnLoaded;
    }
}
