namespace Treatment.Core.Tests.UseCases.CleanAppConfig
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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

        private readonly CleanAppConfigCommandHandler sut;
        private readonly ICleanSingleAppConfig cleanSingleAppConfig;
        private readonly CleanAppConfigCommand cleanAppConfigCommand;
        private readonly StringBuilder actionsHappened;
        private readonly FakeFileSystem fs;

        public CleanAppConfigCommandHandlerTest()
        {
            fs = new FakeFileSystem();
            actionsHappened = new StringBuilder();
            cleanAppConfigCommand = new CleanAppConfigCommand(DIR);

            var fileSearcher = A.Fake<IFileSearch>();
            var sourceControl = A.Fake<IReadOnlySourceControl>();

            A.CallTo(() => fileSearcher.FindFilesIncludingSubdirectories(DIR, "*.csproj"))
             .Invokes(call => actionsHappened.AppendLine(call.ToString()))
             .ReturnsLazily(call => fs.GetFiles().Where(x => x.EndsWith(".csproj", true, CultureInfo.InvariantCulture)).ToArray());

            A.CallTo(() => fileSearcher.FindFilesIncludingSubdirectories(DIR, "app.config"))
             .Invokes(call => actionsHappened.AppendLine(call.ToString()))
             .ReturnsLazily(call => fs.GetFiles().Where(x => x.EndsWith("app.config", false, CultureInfo.InvariantCulture)).ToArray());

            A.CallTo(() => fileSearcher.FindFilesIncludingSubdirectories(DIR, "App.config"))
             .Invokes(call => actionsHappened.AppendLine(call.ToString()))
             .ReturnsLazily(call => fs.GetFiles().Where(x => x.EndsWith("App.config", false, CultureInfo.InvariantCulture)).ToArray());

            A.CallTo(() => sourceControl.GetFileStatus(A<string>._))
             .Invokes(call => actionsHappened.AppendLine(call.ToString()))
             .ReturnsLazily(call => fs.GetFileState(call.Arguments[0].ToString()));

            cleanSingleAppConfig = A.Fake<ICleanSingleAppConfig>();
            A.CallTo(() => cleanSingleAppConfig.ExecuteAsync(A<string>._, A<string>._))
             .Invokes(call => actionsHappened.AppendLine(call.ToString()));

            sut = new CleanAppConfigCommandHandler(
                                                    fileSearcher,
                                                    sourceControl,
                                                    cleanSingleAppConfig);
        }

        /*
        [Fact]
        public void testNameTest()
        {
            // arrange
            _fs.Add(DIR + "/a/file1.csproj", FileStatus.Unchanged);
            _fs.Add(DIR + "/a/app.config", FileStatus.Unchanged);
            _fs.Add(DIR + "/b/file1.csproj", FileStatus.Unchanged);
            _fs.Add(DIR + "/b/App.config", FileStatus.Unchanged);
            _fs.Add(DIR + "/c/file1.csproj", FileStatus.Unchanged);
            _fs.Add(DIR + "/d/App.config", FileStatus.Unchanged);

            // act
            _sut.Execute(_cleanAppConfigCommand);

            // assert
            Approvals.Verify(_actionsHappened);
        }
        */

        [Fact]
        public async Task Execute_WhenCsProjIsModifiedAndAppConfigIsNew_ExecutesCleanSingleAppConfigTest()
        {
            // arrange
            fs.Add(DIR + "/a/file1.csproj", FileStatus.Modified);
            fs.Add(DIR + "/a/app.config", FileStatus.New);

            // act
            await sut.ExecuteAsync(cleanAppConfigCommand);

            // assert
            Approvals.Verify(actionsHappened);
        }

        [Theory]
        [MemberData(nameof(FileStatesExceptNew))]
        public async Task Execute_WhenCsProjIsModifiedAndAppConfigIsNotNew_DoesNotExecuteCleanSingleAppConfigTest(FileStatus status)
        {
            // arrange
            fs.Add(DIR + "/a/file1.csproj", FileStatus.Modified);
            fs.Add(DIR + "/a/app.config", status);

            // act
            await sut.ExecuteAsync(cleanAppConfigCommand);

            // assert
            A.CallTo(cleanSingleAppConfig).MustNotHaveHappened();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        // Justification: Used by xUnit MemberData.
        #pragma warning disable SA1201 // Elements must appear in the correct order
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
        #pragma warning restore SA1201 // Elements must appear in the correct order
    }
}
