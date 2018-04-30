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
    [SuppressMessage("ReSharper", "RedundantNameQualifier",
        Justification = "Extra information that the System.Console.XXX method is shimmed.")]
    public class ConsoleAdapterTest
    {
        private readonly ConsoleAdapter _sut;
        private readonly List<string> _shimResult;

        public ConsoleAdapterTest()
        {
            _sut = ConsoleAdapter.Instance;
            _shimResult = new List<string>(10);
        }

        [Fact]
        public void WriteLine_ShouldCallSystemConsoleWritelineTest()
        {
            // arrange
            var consoleShim = Shim.Replace(() => System.Console.WriteLine(Is.A<string>()))
                                  .With((string s) => _shimResult.Add("shimmed: " + s));

            // act
            PoseContext.Isolate(() => _sut.WriteLine("monkey"), consoleShim);

            // assert
            _shimResult.Should().BeEquivalentTo("shimmed: monkey");
        }

        [Fact]
        public void WriteLine_ShouldCallSystemConsoleWritelineTest2()
        {
            // arrange
            var consoleShim = Shim.Replace(() => System.Console.WriteLine())
                                  .With(() => _shimResult.Add("called"));

            // act
            PoseContext.Isolate(() => _sut.WriteLine(), consoleShim);

            // assert
            _shimResult.Should().BeEquivalentTo("called");
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
                                                               () => result = _sut.ReadKey(),
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