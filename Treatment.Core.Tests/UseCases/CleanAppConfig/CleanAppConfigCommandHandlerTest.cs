namespace Treatment.Core.Tests.UseCases.CleanAppConfig
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using ApprovalTests;

    using EnumsNET;

    using FakeItEasy;

    using JetBrains.Annotations;

    using Treatment.Contract.Commands;
    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Contract.Plugin.SourceControl;
    using Treatment.Core.UseCases.CleanAppConfig;

    using Xunit;

    public class CleanAppConfigCommandHandlerTest
    {
        private const string DIR = "main/directory";

        private CleanAppConfigCommandHandler _sut;
        private IFileSearch _fileSearcher;
        private IReadOnlySourceControl _sourceControl;
        private ICleanSingleAppConfig _cleanSingleAppConfig;
        private CleanAppConfigCommand _command;
        private StringBuilder _actionsHappened;
        private FakeFileSystem _fs;

        public CleanAppConfigCommandHandlerTest()
        {
            _fs = new FakeFileSystem();
            _actionsHappened = new StringBuilder();
            _command = new CleanAppConfigCommand(DIR);

            _fileSearcher = A.Fake<IFileSearch>();
            _sourceControl = A.Fake<IReadOnlySourceControl>();

            A.CallTo(() => _fileSearcher.FindFilesIncludingSubdirectories(DIR, "*.csproj"))
             .Invokes(call => _actionsHappened.AppendLine(call.ToString()))
             .ReturnsLazily(call => _fs.GetFiles().Where(x => x.EndsWith(".csproj", true, CultureInfo.InvariantCulture)).ToArray());

            A.CallTo(() => _fileSearcher.FindFilesIncludingSubdirectories(DIR, "app.config"))
             .Invokes(call => _actionsHappened.AppendLine(call.ToString()))
             .ReturnsLazily(call => _fs.GetFiles().Where(x => x.EndsWith("app.config", false, CultureInfo.InvariantCulture)).ToArray());

            A.CallTo(() => _fileSearcher.FindFilesIncludingSubdirectories(DIR, "App.config"))
             .Invokes(call => _actionsHappened.AppendLine(call.ToString()))
             .ReturnsLazily(call => _fs.GetFiles().Where(x => x.EndsWith("App.config", false, CultureInfo.InvariantCulture)).ToArray());

            A.CallTo(() => _sourceControl.GetFileStatus(A<string>._))
             .Invokes(call => _actionsHappened.AppendLine(call.ToString()))
             .ReturnsLazily(call => _fs.GetFileState(call.Arguments[0].ToString()));

            _cleanSingleAppConfig = A.Fake<ICleanSingleAppConfig>();
            A.CallTo(() => _cleanSingleAppConfig.Execute(A<string>._, A<string>._))
             .Invokes(call => _actionsHappened.AppendLine(call.ToString()));

            _sut = new CleanAppConfigCommandHandler(
                                                    _fileSearcher,
                                                    _sourceControl,
                                                    _cleanSingleAppConfig);
        }

        // [Fact]
        // public void testNameTest()
        // {
        //     // arrange
        //     _fs.Add(DIR + "/a/file1.csproj", FileStatus.Unchanged);
        //     _fs.Add(DIR + "/a/app.config", FileStatus.Unchanged);
        //     _fs.Add(DIR + "/b/file1.csproj", FileStatus.Unchanged);
        //     _fs.Add(DIR + "/b/App.config", FileStatus.Unchanged);
        //     _fs.Add(DIR + "/c/file1.csproj", FileStatus.Unchanged);
        //     _fs.Add(DIR + "/d/App.config", FileStatus.Unchanged);
        //
        //     // act
        //     _sut.Execute(_command);
        //
        //     // assert
        //     Approvals.Verify(_actionsHappened);
        // }


        [Fact]
        public void Execute_WhenCsProjIsModifiedAndAppConfigIsNew_ExecutesCleanSingleAppConfigTest()
        {
            // arrange
            _fs.Add(DIR + "/a/file1.csproj", FileStatus.Modified);
            _fs.Add(DIR + "/a/app.config", FileStatus.New);

            // act
            _sut.Execute(_command);

            // assert
            Approvals.Verify(_actionsHappened);
        }

        [Theory]
        [MemberData(nameof(FileStatesExceptNew))]
        public void Execute_WhenCsProjIsModifiedAndAppConfigIsNotNew_DoesNotExecuteCleanSingleAppConfigTest(FileStatus status)
        {
            // arrange
            _fs.Add(DIR + "/a/file1.csproj", FileStatus.Modified);
            _fs.Add(DIR + "/a/app.config", status);

            // act
            _sut.Execute(_command);

            // assert
            A.CallTo(_cleanSingleAppConfig).MustNotHaveHappened();
        }


        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<object[]> FileStatesExceptNew
        {
            [UsedImplicitly]
            get
            {
                return Enums.GetMembers<FileStatus>()
                            .Where(item => item.Value != FileStatus.New)
                            .Select(item => new object[] { item.Value });
            }
        }
    }
}