﻿namespace Treatment.UI.Tests.View
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;

    using ApprovalTests.Reporters;
    using ApprovalTests.Wpf;
    using FakeItEasy;
    using TestHelper;
    using Treatment.Contract;
    using Treatment.UI.Core.View;
    using Treatment.UI.Core.ViewModel;

    public class MainWindowTest
    {
        [ConditionalHostFact(TestHostMode.Skip, TestHost.AppVeyor)]
        [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void MainWindow_ShouldShowProjects_WhenInitializedWithTwo()
        {
            WpfApprovals.Verify(() =>
            {
                var mainWindowViewModel = A.Fake<IMainWindowViewModel>();
                var statusViewModel = A.Fake<IStatusViewModel>();
                var commandDispatcher = A.Fake<ICommandDispatcher>();
                var projectCollection = A.Fake<IProjectCollectionViewModel>();
                var projects = new ObservableCollection<ProjectViewModel>(new List<ProjectViewModel>(2)
                {
                    new ProjectViewModel("project A", "C:\\ProjA\\", commandDispatcher),
                    new ProjectViewModel("Project B", "D:\\Proj B\\", commandDispatcher),
                });

                A.CallTo(() => statusViewModel.ConfigFilename).Returns("D:\\config.txt");
                A.CallTo(() => projectCollection.Projects).Returns(projects);
                A.CallTo(() => mainWindowViewModel.StatusViewModel).Returns(statusViewModel);
                A.CallTo(() => mainWindowViewModel.ProjectCollection).Returns(projectCollection);

                return new MainWindow(mainWindowViewModel);
            });
        }
    }
}
