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
        private const string AppConfigFilename = "app.config";
        private readonly IFileSystem fileSystem;
        private readonly CleanSingleAppConfig sut;

        public CleanSingleAppConfigTest()
        {
            fileSystem = A.Fake<IFileSystem>();
            sut = new CleanSingleAppConfig(fileSystem);
        }

        [Fact]
        public async Task ExecuteShouldFixCsProjFileAndDeleteAppconfigFileTest()
        {
            // arrange
            const string csprojFilename = "FileWithRelativeHintPath.txt";
            string outputContent = null;
            A.CallTo(() => fileSystem.OpenRead(csprojFilename, false)).Returns(ResourceFile.OpenRead(csprojFilename));
            A.CallTo(() => fileSystem.SaveContentAsync(csprojFilename, A<Stream>._))
             .Invokes(call =>
                      {
                          var outputStream = call.Arguments[1] as MemoryStream;
                          outputContent = System.Text.Encoding.UTF8.GetString(outputStream?.ToArray() ?? new byte[0]);
                      });

            // act
            await sut.ExecuteAsync(csprojFilename, AppConfigFilename);

            // assert
            A.CallTo(() => fileSystem.DeleteFile(AppConfigFilename)).MustHaveHappenedOnceExactly();
            Approvals.Verify(outputContent);
        }

        [Fact]
        public async Task ExecuteAsyncShouldNotDeleteAppConfigFileWhenCsprojFileDidNotChangeTest()
        {
            // arrange
            const string csprojFilename = "FileWithRelativeHintPathWithoutAppConfig.txt";
            A.CallTo(() => fileSystem.OpenRead(csprojFilename, false)).Returns(ResourceFile.OpenRead(csprojFilename));

            // act
            await sut.ExecuteAsync(csprojFilename, AppConfigFilename);

            // assert
            A.CallTo(() => fileSystem.DeleteFile(AppConfigFilename)).MustNotHaveHappened();
            A.CallTo(() => fileSystem.SaveContentAsync(csprojFilename, A<Stream>._)).MustNotHaveHappened();
        }
    }
}
