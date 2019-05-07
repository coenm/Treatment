namespace Treatment.TestAutomation.Contract.Interfaces.Events
{
    using System;

    using JetBrains.Annotations;

    [PublicAPI]
    public class NewControlCreated : TestElementEventBase
    {
        public Type Interface { get; set; }
    }
}
