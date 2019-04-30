namespace TestAgent.RequestHandlers
{
    using System;

    using JetBrains.Annotations;

    public interface ITestAgent
    {
        event EventHandler MessageBoxShown;
        event EventHandler MessageBoxClosed;
        event EventHandler ApplicationStarted;
        event EventHandler ApplicationClosed;

        [CanBeNull]
        IMessageBox MessageBox { get; }

//        [CanBeNull]
//        ITestAutomationView MainView { get; }

        byte[] GetFileContent(string filename);

        bool UpdateAppConfig(string filename, string key, string value);

        bool TryDeleteFile(string filename);

        string TakeScreenShot();

        bool StartSut();

        bool StopSut();
    }
}
