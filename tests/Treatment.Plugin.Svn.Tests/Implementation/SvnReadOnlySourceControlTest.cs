﻿namespace Treatment.Plugin.Svn.Tests.Implementation
{
    using System.Linq;
    using System.Runtime.CompilerServices;

    using ApprovalTests;
    using ApprovalTests.Reporters;
    using FluentAssertions;
    using TestHelper;
    using Treatment.Contract.Plugin.SourceControl;
    using Treatment.Helpers.FileSystem;
    using Treatment.Plugin.Svn.Implementation;
    using Xunit;

    public class SvnReadOnlySourceControlTest
    {
        private readonly SvnReadOnlySourceControl sut;
        private readonly string expectedSvnRootDirectory;

        public SvnReadOnlySourceControlTest()
        {
            expectedSvnRootDirectory = TestEnvironment.GetFullPath("tests", "TestData", "SvnCheckout");
            sut = new SvnReadOnlySourceControl(OsFileSystem.Instance);
        }

        [Theory]
        [InlineData("tests\\TestData", false)] // dir exists but is not 'source controlled' using svn.
        [InlineData("tests\\TestData\\SvnCheckout\\txt\\file1.txt", true)]
        [InlineData("tests\\TestData\\SvnCheckout\\txt\\", true)]
        [InlineData("tests\\TestData\\SvnCheckout\\txt\\nonExistingDirectory", false)] // inside svn root but directory does not exists.
        public void TryGetSvnRoot_ShouldReturnRoot_WhenExistTest(string path, bool expectedRepoFound)
        {
            // arrange
            var fullPath = TestEnvironment.GetFullPath(path);

            // act
            var result = sut.TryGetSvnRoot(fullPath, out var root);

            // assert
            if (!expectedRepoFound)
            {
                result.Should().BeFalse();
                root.Should().BeNull();
            }
            else
            {
                result.Should().BeTrue();
                DirectoryShouldMatch(root, expectedSvnRootDirectory);
            }
        }

        [Theory]
        [InlineData("tests\\TestData\\fileOutsideRepo.txt", FileStatus.Unknown)]
        [InlineData("tests\\TestData\\fileOutsideRepoNotExist.txt", FileStatus.NotExist)]
        [InlineData("tests\\TestData\\SvnCheckout\\txt\\fileInsideRepoNotExists.txt", FileStatus.NotExist)]
        [InlineData("tests\\TestData\\SvnCheckout\\txt\\file1.txt", FileStatus.Unchanged)]
        [InlineData("tests\\TestData\\SvnCheckout\\txt\\dir1\\file1.txt", FileStatus.Modified)]
        [InlineData("tests\\TestData\\SvnCheckout\\txt\\dir1\\file2.txt", FileStatus.Modified)] // renamed from file2.txt -> file2.renamed.txt. Marked as modified. Not sure if this is what i want.
        [InlineData("tests\\TestData\\SvnCheckout\\txt\\dir1\\file2.renamed.txt", FileStatus.New)]
        [InlineData("tests\\TestData\\SvnCheckout\\txt\\dir1\\new file marked as added.txt", FileStatus.New)]
        [InlineData("tests\\TestData\\SvnCheckout\\txt\\dir1\\new file not marked as added.txt", FileStatus.New)]
        [InlineData("tests\\TestData\\SvnCheckout\\txt\\notCommittedDir\\file1.txt", FileStatus.New)]
        [InlineData("tests\\TestData\\SvnCheckout\\txt\\notCommittedDir\\fileNotExist.txt", FileStatus.NotExist)]
        public void GetFileStatus_ShouldReturnRoot_WhenExistTest(string path, FileStatus expectedState)
        {
            // arrange
            var fullPath = TestEnvironment.GetFullPath(path);

            // act
            var result = sut.GetFileStatus(fullPath);

            // assert
            result.Should().Be(expectedState);
        }

        [Theory]
        [InlineData("tests\\TestData\\SvnCheckout\\txt\\file1.txt", "")] // unchanged.
        [InlineData("tests\\TestData\\SvnCheckout\\txt\\notCommittedDir\\file1.txt", null)]
        public void GetModifications_ShouldReturnRoot_WhenExistTest(string path, string expectedState)
        {
            // arrange
            var fullPath = TestEnvironment.GetFullPath(path);

            // act
            var result = sut.GetModifications(fullPath);

            // assert
            result.Should().Be(expectedState);
        }

        [Fact]
        [UseReporter(typeof(TortoiseDiffReporter), typeof(XUnit2Reporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void GetModifications_ShouldReturnModifiedContentTest()
        {
            // arrange
            var fullPath = TestEnvironment.GetFullPath("tests", "TestData", "SvnCheckout", "txt", "dir1", "file1.txt");

            // act
            var result = sut.GetModifications(fullPath);

            // assert
            var sanitizedResult = Sanitizer(result);
            Approvals.Verify(sanitizedResult);
        }

        private static void DirectoryShouldMatch(string directory1, string directory2)
        {
            directory1.Should().NotBeNullOrWhiteSpace();
            directory2.Should().NotBeNullOrWhiteSpace();

            // make drive uppercase
            var dir1 = MakeDrivePathUppercase(directory1);
            var dir2 = MakeDrivePathUppercase(directory2);

            dir1.Should().Be(dir2);
        }

        private static string Sanitizer(string input)
        {
            var rootPath = TestEnvironment.GetFullPath().Replace("\\", "/").ToCharArray();

            rootPath[0] = rootPath[0].ToString().ToUpper().First();
            var rootPathCapitalDrive = new string(rootPath);

            rootPath[0] = rootPath[0].ToString().ToLower().First();
            var rootPathSmallDrive = new string(rootPath);

            return input
                .Replace(rootPathCapitalDrive, "ROOTPATH")
                .Replace(rootPathSmallDrive, "ROOTPATH");
        }

        private static string MakeDrivePathUppercase(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;

            var pathArray = path.ToCharArray();
            pathArray[0] = pathArray[0].ToString().ToUpper().First();
            return new string(pathArray);
        }
    }
}
