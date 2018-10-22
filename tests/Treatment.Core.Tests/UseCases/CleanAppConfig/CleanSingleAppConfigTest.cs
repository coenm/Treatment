namespace Treatment.Core.Tests.UseCases.CleanAppConfig
{
    using System.IO;
    using System.Threading.Tasks;

    using ApprovalTests;

    using FakeItEasy;

    using Treatment.Core.Interfaces;
    using Treatment.Core.Tests.Resources;
    using Treatment.Core.UseCases.CleanAppConfig;

    using Xunit;

    public class CleanSingleAppConfigTest
    {
        private const string APPCONFIG_FILENAME = "app.config";
        private readonly IFileSystem _fileSystem;
        private readonly CleanSingleAppConfig _sut;

        public CleanSingleAppConfigTest()
        {
            _fileSystem = A.Fake<IFileSystem>();
            _sut = new CleanSingleAppConfig(_fileSystem);
        }

        [Fact]
        public async Task ExecuteShouldFixCsProjFileAndDeleteAppconfigFileTest()
        {
            // arrange
            const string CSPROJ_FILENAME = "FileWithRelativeHintPath.txt";
            string outputContent = null;
            A.CallTo(() => _fileSystem.ReadFile(CSPROJ_FILENAME)).Returns(ResourceFile.OpenRead(CSPROJ_FILENAME));
            A.CallTo(() => _fileSystem.SaveContentAsync(CSPROJ_FILENAME, A<Stream>._))
             .Invokes(call =>
                      {
                          var outputStream = call.Arguments[1] as MemoryStream;
                          outputContent = System.Text.Encoding.UTF8.GetString(outputStream?.ToArray() ?? new byte[0]);
                      });

            // act
            await _sut.ExecuteAsync(CSPROJ_FILENAME, APPCONFIG_FILENAME);

            // assert
            A.CallTo(() => _fileSystem.DeleteFile(APPCONFIG_FILENAME)).MustHaveHappenedOnceExactly();
            Approvals.Verify(outputContent);
        }

        [Fact]
        public async Task ExecuteAsyncShouldNotDeleteAppConfigFileWhenCsprojFileDidNotChangeTest()
        {
            // arrange
            const string CSPROJ_FILENAME = "FileWithRelativeHintPathWithoutAppConfig.txt";
            A.CallTo(() => _fileSystem.ReadFile(CSPROJ_FILENAME)).Returns(ResourceFile.OpenRead(CSPROJ_FILENAME));

            // act
            await _sut.ExecuteAsync(CSPROJ_FILENAME, APPCONFIG_FILENAME);

            // assert
            A.CallTo(() => _fileSystem.DeleteFile(APPCONFIG_FILENAME)).MustNotHaveHappened();
            A.CallTo(() => _fileSystem.SaveContentAsync(CSPROJ_FILENAME, A<Stream>._)).MustNotHaveHappened();
        }
    }
}