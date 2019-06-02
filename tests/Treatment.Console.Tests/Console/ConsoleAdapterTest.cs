namespace Treatment.Console.Tests.Console
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;
    using Pose;
    using Treatment.Console.Console;
    using Xunit;

    /// <summary>
    /// Testing the ConsoleAdapter using Shims to assert the right System.Console.x method is called.
    /// Somewhat superfluous but nice to experiment with the Pose library.
    /// Not all method are tested...
    /// </summary>
    [SuppressMessage("ReSharper", "RedundantNameQualifier", Justification = "Extra information that the System.Console.XXX method is shimmed.")]
    public class ConsoleAdapterTest
    {
        private readonly ConsoleAdapter sut;
        private readonly List<string> shimResult;

        public ConsoleAdapterTest()
        {
            sut = ConsoleAdapter.Instance;
            shimResult = new List<string>(10);
        }

        [Fact]
        public void WriteLine_ShouldCallSystemConsoleWriteLineTest()
        {
            // arrange
            var consoleShim = Shim.Replace(() => System.Console.WriteLine(Is.A<string>()))
                                  .With((string s) => shimResult.Add("shimmed: " + s));

            // act
            PoseContext.Isolate(() => sut.WriteLine("monkey"), consoleShim);

            // assert
            shimResult.Should().BeEquivalentTo("shimmed: monkey");
        }

        [Fact]
        public void WriteLine_ShouldCallSystemConsoleWriteLineTest2()
        {
            // arrange
            var consoleShim = Shim.Replace(() => System.Console.WriteLine())
                                  .With(() => shimResult.Add("called"));

            // act
            PoseContext.Isolate(() => sut.WriteLine(), consoleShim);

            // assert
            shimResult.Should().BeEquivalentTo("called");
        }

        [Fact]
        public async Task ReadKey_ShouldCallShimmedConsoleReadKeyMethodTest()
        {
            // arrange
            var keyPressTask = new TaskCompletionSource<ConsoleKeyInfo>();
            var mreConsoleReadingKey = new ManualResetEventSlim(false);
            var consoleShim = Shim.Replace(() => System.Console.ReadKey())
                                  .With(() =>
                                        {
                                            // simulating a wait for a keypress
                                            mreConsoleReadingKey.Set();
                                            return keyPressTask.Task.GetAwaiter().GetResult();
                                        });

            // act
            var result = default(ConsoleKeyInfo);
            var readKeyTask = Task.Run(() =>
                                           PoseContext.Isolate(
                                                               () => result = sut.ReadKey(),
                                                               consoleShim));

            // wait until the System.Console is 'reading the key'.
            mreConsoleReadingKey.Wait();

            // simulate keypress
            keyPressTask.TrySetResult(new ConsoleKeyInfo('e', ConsoleKey.A, true, false, true));

            await readKeyTask.ConfigureAwait(false);

            // assert
            result.Should().Be(new ConsoleKeyInfo('e', ConsoleKey.A, true, false, true));
        }
    }
}
