 namespace Treatment.Core.Tests.FileSystem
 {
    using FakeItEasy;
    using FluentAssertions;
    using Treatment.Console;
    using Treatment.Console.Console;
    using Treatment.Console.Decorators;
    using Treatment.Helpers.FileSystem;
    using Xunit;

    public class DryRunFileSystemDecoratorTest
     {
         private readonly DryRunFileSystemDecorator sut;
         private readonly IFileSystem decoratee;

         public DryRunFileSystemDecoratorTest()
         {
             var console = A.Dummy<IConsole>();
             decoratee = A.Fake<IFileSystem>();
             var sanitizer = A.Fake<IRootDirSanitizer>();
             A.CallTo(() => sanitizer.Sanitize(A<string>._)).Returns(string.Empty);

             sut = new DryRunFileSystemDecorator(decoratee, sanitizer, console);
         }

         [Fact]
         public void SaveContent_NeverCallsDecorateeSaveContentTest()
         {
             // arrange
             const string filename = "a.txt";
             const string content = "data";

             // act
             sut.SaveContent(filename, content);

             // assert
             A.CallTo(() => decoratee.SaveContent(filename, content)).MustNotHaveHappened();
         }

         [Fact]
         public void GetFileContent_ShouldReturnDecorateeGetFileContentTest()
         {
             // arrange
             const string filename = "a.txt";
             const string content = "data";
             A.CallTo(() => decoratee.GetFileContent(filename)).Returns(content);

             // act
             var result = sut.GetFileContent(filename);

             // assert
             result.Should().Be(content);
             A.CallTo(() => decoratee.GetFileContent(filename)).MustHaveHappenedOnceExactly();
         }
     }
 }
