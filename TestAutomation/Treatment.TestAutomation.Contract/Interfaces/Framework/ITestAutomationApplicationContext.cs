namespace Treatment.TestAutomation.Contract.Interfaces.Framework
{
    using System;
    using JetBrains.Annotations;

    public interface ITestAutomationApplicationContext
    {
        [CanBeNull]
        ITestAutomationView MainView { get; }

    }
}

