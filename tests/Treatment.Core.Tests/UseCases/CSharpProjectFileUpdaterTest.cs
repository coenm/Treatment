namespace Treatment.Core.Tests.UseCases
{
    using System.IO;
    using System.Runtime.CompilerServices;

    using ApprovalTests;

    using FluentAssertions;

    using Treatment.Core.Tests.Resources;
    using Treatment.Core.UseCases;

    using Xunit;

    public class CSharpProjectFileUpdaterTest
    {
        private const string FILENAME = "FileWithRelativeHintPath.txt";

        [Fact]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void RemoveEmptyItemGroups_RemovesEmptyGroupsTest()
        {
            // arrange
            using (var inputStream = ResourceFile.OpenRead(FILENAME))
            using (var outputStream = new MemoryStream())
            {
                var sut = CSharpProjectFileUpdater.Create(inputStream)
                                                  .RemoveAppConfig();

                // act
                sut.RemoveEmptyItemGroups();

                // assert
                sut.Save(outputStream);
                Approvals.Verify(GetContent(outputStream));
            }
        }

        [Fact]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void RemoveAppConfig_RemovedAppConfigTest()
        {
            // arrange
            using (var inputStream = ResourceFile.OpenRead(FILENAME))
            using (var outputStream = new MemoryStream())
            {
                // act
                CSharpProjectFileUpdater.Create(inputStream)
                                 .RemoveAppConfig()
                                 .Save(outputStream);

                // assert
                Approvals.Verify(GetContent(outputStream));
            }
        }

        private static string GetContent(Stream stream)
        {
            stream.Should().NotBeNull();
            stream.CanSeek.Should().BeTrue();
            stream.CanRead.Should().BeTrue();

            stream.Position = 0;
            using (var sr = new StreamReader(stream))
            {
                return sr.ReadToEnd();
            }
        }
    }
}