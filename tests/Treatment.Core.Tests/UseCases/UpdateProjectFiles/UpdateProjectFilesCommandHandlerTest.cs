namespace Treatment.Core.Tests.UseCases.UpdateProjectFiles
{
    using FakeItEasy;

    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Core.Interfaces;
    using Treatment.Core.Tests.Resources;
    using Treatment.Core.UseCases.UpdateProjectFiles;

    using Xunit;

    public class UpdateProjectFilesCommandHandlerTests
    {
        private readonly UpdateProjectFilesCommandHandlerImplementation _sut;
        private readonly IFileSystem _filesystem;

        public UpdateProjectFilesCommandHandlerTests()
        {
            var filesearcher = A.Dummy<IFileSearch>();
            _filesystem = A.Fake<IFileSystem>();
            A.CallTo(() => _filesystem.GetFileContent(A<string>._))
             .ReturnsLazily(call =>
                            {
                                var filename = call.Arguments[0] as string;
                                return ResourceFile.GetContent(filename);
                            });

            _sut = new UpdateProjectFilesCommandHandlerImplementation(_filesystem, filesearcher);
        }

        [Fact]
        public void FixFileTest()
        {
            // arrange
            const string FILENAME = "FileWithRelativeHintPath.txt";
            var expectedContent = ResourceFile.GetContent("FileWithRelativeHintPathFixed.txt");

            // act
            _sut.FixSingleFile(FILENAME);

            // assert
            A.CallTo(() => _filesystem.SaveContent(FILENAME, expectedContent))
             .MustHaveHappenedOnceExactly();
        }
    }
}