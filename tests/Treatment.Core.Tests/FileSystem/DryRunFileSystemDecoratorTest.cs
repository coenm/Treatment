 namespace Treatment.Core.Tests.FileSystem
 {
     using FakeItEasy;

     using FluentAssertions;

     using Treatment.Console;
     using Treatment.Console.Console;
     using Treatment.Console.Decorators;
     using Treatment.Core.Interfaces;

     using Xunit;

     public class DryRunFileSystemDecoratorTest
     {
         private readonly DryRunFileSystemDecorator sut;
         private readonly IFileSystem decoratee;
         private readonly IConsole console;

         public DryRunFileSystemDecoratorTest()
         {
             console = A.Dummy<IConsole>();
             decoratee = A.Fake<IFileSystem>();
             var sanitizer = A.Fake<IRootDirSanitizer>();
             A.CallTo(() => sanitizer.Sanitize(A<string>._)).Returns(string.Empty);

             sut = new DryRunFileSystemDecorator(decoratee, sanitizer, console);
         }

         [Fact]
         public void SaveContent_NeverCallsDecorateeSaveContentTest()
         {
             // arrange
             const string FILENAME = "a.txt";
             const string CONTENT = "data";

             // act
             sut.SaveContent(FILENAME, CONTENT);

             // assert
             A.CallTo(() => decoratee.SaveContent(FILENAME, CONTENT)).MustNotHaveHappened();
         }

         [Fact]
         public void GetFileContent_ShouldReturnDecorateeGetFileContentTest()
         {
             // arrange
             const string FILENAME = "a.txt";
             const string CONTENT = "data";
             A.CallTo(() => decoratee.GetFileContent(FILENAME)).Returns(CONTENT);

             // act
             var result = sut.GetFileContent(FILENAME);

             // assert
             result.Should().Be(CONTENT);
             A.CallTo(() => decoratee.GetFileContent(FILENAME)).MustHaveHappenedOnceExactly();
         }
     }
 }
