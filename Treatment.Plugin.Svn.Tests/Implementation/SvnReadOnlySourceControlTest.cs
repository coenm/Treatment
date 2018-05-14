namespace Treatment.Plugin.Svn.Tests.Implementation
{
    using System.Runtime.CompilerServices;

    using ApprovalTests;
    using ApprovalTests.Reporters;

    using FluentAssertions;

    using TestHelper;

    using Treatment.Contract.Plugin.SourceControl;
    using Treatment.Core.FileSystem;
    using Treatment.Plugin.Svn.Implementation;

    using Xunit;

    public class SvnReadOnlySourceControlTest
    {
        private readonly SvnReadOnlySourceControl _sut;
        private readonly string _expectedSvnRootDirectory;

        public SvnReadOnlySourceControlTest()
        {
            _expectedSvnRootDirectory = TestEnvironment.GetFullPath("TestData", "SvnCheckout");
            _sut = new SvnReadOnlySourceControl(OsFileSystem.Instance);
        }

        [Theory]
        [InlineData("TestData", false)] // dir exists but is not 'source controlled' using svn.
        [InlineData("TestData\\SvnCheckout\\txt\\file1.txt", true)]
        [InlineData("TestData\\SvnCheckout\\txt\\", true)]
        [InlineData("TestData\\SvnCheckout\\txt\\nonExistingDirectory", false)] // inside svn root but directory does not exists.
        public void TryGetSvnRoot_ShouldReturnRoot_WhenExistTest(string path, bool expectedRepoFound)
        {
            // arrange
            var fullPath = TestEnvironment.GetFullPath(path);

            // act
            var result = _sut.TryGetSvnRoot(fullPath, out var root);

            // assert
            if (!expectedRepoFound)
            {
                result.Should().BeFalse();
                root.Should().BeNull();
            }
            else
            {
                result.Should().BeTrue();
                root.Should().Be(_expectedSvnRootDirectory);
            }
        }

        [Theory]
        [InlineData("TestData\\fileOutsideRepo.txt", FileStatus.Unknown)]
        [InlineData("TestData\\fileOutsideRepoNotExist.txt", FileStatus.NotExist)]
        [InlineData("TestData\\SvnCheckout\\txt\\fileInsideRepoNotExists.txt", FileStatus.NotExist)]
        [InlineData("TestData\\SvnCheckout\\txt\\file1.txt", FileStatus.Unchanged)]
        [InlineData("TestData\\SvnCheckout\\txt\\dir1\\file1.txt", FileStatus.Modified)]
        [InlineData("TestData\\SvnCheckout\\txt\\dir1\\file2.txt", FileStatus.Modified)] // renamed from file2.txt -> file2.renamed.txt. Marked as modiefied. Not sure if this is what i want.
        [InlineData("TestData\\SvnCheckout\\txt\\dir1\\file2.renamed.txt", FileStatus.New)]
        [InlineData("TestData\\SvnCheckout\\txt\\dir1\\new file marked as added.txt", FileStatus.New)]
        [InlineData("TestData\\SvnCheckout\\txt\\dir1\\new file not marked as added.txt", FileStatus.New)]
        [InlineData("TestData\\SvnCheckout\\txt\\notCommittedDir\\file1.txt", FileStatus.New)]
        [InlineData("TestData\\SvnCheckout\\txt\\notCommittedDir\\fileNotExist.txt", FileStatus.NotExist)]
        public void GetFileStatus_ShouldReturnRoot_WhenExistTest(string path, FileStatus expectedState)
        {
            // arrange
            var fullPath = TestEnvironment.GetFullPath(path);

            // act
            var result = _sut.GetFileStatus(fullPath);

            // assert
            result.Should().Be(expectedState);
        }

        [Theory]
        [InlineData("TestData\\SvnCheckout\\txt\\file1.txt", "")] // unchanged.
        [InlineData("TestData\\SvnCheckout\\txt\\notCommittedDir\\file1.txt", null)]
        public void GetModifications_ShouldReturnRoot_WhenExistTest(string path, string expectedState)
        {
            // arrange
            var fullPath = TestEnvironment.GetFullPath(path);

            // act
            var result = _sut.GetModifications(fullPath);

            // assert
            result.Should().Be(expectedState);
        }

        [Fact]
        [UseReporter(typeof(TortoiseDiffReporter), typeof(XUnit2Reporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void GetModifications_ShouldReturnModifiedContentTest()
        {
            // arrange
            var fullPath = TestEnvironment.GetFullPath("TestData", "SvnCheckout", "txt", "dir1", "file1.txt");

            // act
            var result = _sut.GetModifications(fullPath);

            // assert
            var sanitizedResult = Sanitizer(result);
            Approvals.Verify(sanitizedResult);
        }

        private static string Sanitizer(string input)
        {
            var rootPath = TestEnvironment.GetFullPath()
                                          .Replace("\\", "/");
            return input.Replace(rootPath, "ROOTPATH");
        }
    }
}