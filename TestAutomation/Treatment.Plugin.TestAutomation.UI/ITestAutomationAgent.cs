﻿namespace Treatment.Plugin.TestAutomation.UI
{
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Treatment.Plugin.TestAutomation.UI.Adapters;
    using Treatment.Plugin.TestAutomation.UI.Interfaces;

    internal interface ITestAutomationAgent
    {
        void AddPopupViewAndInitialize(SettingWindowAdapter settingWindow);

        void RegisterAndInitializeApplication([NotNull] ITestAutomationApplication application);

        void RegisterAndInitializeMainView(MainWindowAdapter mainWindowAdapter);

        void RegisterWorker(Task workerTask);
    }
}