namespace Treatment.Core.Tests
{
    using FakeItEasy;

    using Treatment.Core.Interfaces;
    using Treatment.Core.Tests.Resources;

    using Xunit;

    public class RelativePathInCsProjFixerTests
    {
        private readonly RelativePathInCsProjFixer _sut;
        private readonly IFileSystem _filesystem;
        private readonly IFileSearch _filesearcher;

        public RelativePathInCsProjFixerTests()
        {
            _filesearcher = A.Fake<IFileSearch>();
            _filesystem = A.Fake<IFileSystem>();
            A.CallTo(() => _filesystem.GetFileContent(A<string>._))
             .ReturnsLazily(call =>
                            {
                                var filename = call.Arguments[0] as string;
                                return ResourceFile.GetContent(filename);
                            });

            _sut = new RelativePathInCsProjFixer(_filesystem, _filesearcher);
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