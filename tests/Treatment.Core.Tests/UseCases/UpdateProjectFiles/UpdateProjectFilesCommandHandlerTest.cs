namespace Treatment.Core.Tests.UseCases.UpdateProjectFiles
{
    using FakeItEasy;

    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Core.Interfaces;
    using Treatment.Core.Tests.Resources;
    using Treatment.Core.UseCases.UpdateProjectFiles;

    using Xunit;

    public class UpdateProjectFilesCommandHandlerTest
    {
        private readonly UpdateProjectFilesCommandHandlerImplementation sut;
        private readonly IFileSystem filesystem;

        public UpdateProjectFilesCommandHandlerTest()
        {
            var fileSearcher = A.Dummy<IFileSearch>();
            filesystem = A.Fake<IFileSystem>();
            A.CallTo(() => filesystem.GetFileContent(A<string>._))
             .ReturnsLazily(call =>
                            {
                                var filename = call.Arguments[0] as string;
                                return ResourceFile.GetContent(filename);
                            });

            sut = new UpdateProjectFilesCommandHandlerImplementation(filesystem, fileSearcher);
        }

        [Fact]
        public void FixFileTest()
        {
            // arrange
            const string FILENAME = "FileWithRelativeHintPath.txt";
            var expectedContent = ResourceFile.GetContent("FileWithRelativeHintPathFixed.txt");

            // act
            sut.FixSingleFile(FILENAME);

            // assert
            A.CallTo(() => filesystem.SaveContent(FILENAME, expectedContent))
             .MustHaveHappenedOnceExactly();
        }
    }
}
