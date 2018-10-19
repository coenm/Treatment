namespace Treatment.Console.Tests.E2ETests
{
    using System;
    using System.IO;
    using System.Text;

    using ApprovalTests;

    using FluentAssertions;

    using Treatment.Console.Console;
    using Treatment.Console.Tests.E2ETests.Fakes;

    using Xunit;

    public class ProgramTests
    {
        private readonly FakeBootstrapper _bootstrapper;
        private readonly FakeConsoleAdapter _console;
        private readonly StringBuilder _sb;

        public ProgramTests()
        {
            _console = new FakeConsoleAdapter();

            _bootstrapper = new FakeBootstrapper();
            _bootstrapper.RegisterPostRegisterAction(container => container.RegisterInstance<IConsole>(_console));
            // _bootstrapper.RegisterPostRegisterAction(container => container.RegisterInstance<IFileSystem>(_console));
            // container.RegisterInstance<IFileSystem>(OsFileSystem.Instance);

            _sb = new StringBuilder();
            Console.SetOut(new StringWriter(_sb));
            Console.SetError(new StringWriter(_sb));

            Program.Bootstrapper = _bootstrapper;
        }

        [Fact]
        public void StartWithEmptyParametersDoesNothingTest()
        {
            // arrange

            // act
            var result = Program.Main(string.Empty);

            // assert
            result.Should().Be(-1);
        }

        [Fact]
        public void ListProvidersShouldListFileSystemTest()
        {
            // arrange

            // act
            var result = Program.Main("list-providers");

            // assert
            result.Should().Be(0);
            Approvals.Verify(_console.ToString());
        }
    }
}