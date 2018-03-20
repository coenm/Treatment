namespace Treatment.Core.Tests.FileSystem
{
    using FakeItEasy;

    using FluentAssertions;

    using Treatment.Core.FileSystem;
    using Treatment.Core.Interfaces;

    using Xunit;

    public class DryRunFileSystemDecoratorTest
    {
        private readonly DryRunFileSystemDecorator _sut;
        private readonly IFileSystem _decoratee;

        public DryRunFileSystemDecoratorTest()
        {
            _decoratee = A.Fake<IFileSystem>();
            var sanitizer = A.Fake<IRootDirSanitizer>();
            A.CallTo(() => sanitizer.Sanitize(A<string>._)).Returns(string.Empty);

            _sut = new DryRunFileSystemDecorator(_decoratee, sanitizer);
        }

        [Fact]
        public void SaveContent_NeverCallsDecorateeSaveContentTest()
        {
            // arrange
            const string FILENAME = "a.txt";
            const string CONTENT = "data";

            // act
            _sut.SaveContent(FILENAME, CONTENT);

            // assert
            A.CallTo(() => _decoratee.SaveContent(FILENAME, CONTENT)).MustNotHaveHappened();
        }

        [Fact]
        public void GetFileContent_ShouldReturnDecorateeGetFileContentTest()
        {
            // arrange
            const string FILENAME = "a.txt";
            const string CONTENT = "data";
            A.CallTo(() => _decoratee.GetFileContent(FILENAME)).Returns(CONTENT);

            // act
            var result = _sut.GetFileContent(FILENAME);

            // assert
            result.Should().Be(CONTENT);
            A.CallTo(() => _decoratee.GetFileContent(FILENAME)).MustHaveHappenedOnceExactly();
        }
    }
}