namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using System;

    public interface ITestAutomationView : IDisposable
    {
        Guid Guid { get; }

        void Initialize();
    }
}
