namespace Treatment.Console.Tests.E2ETests
{
    using System;
    using System.IO;
    using System.Text;

    using FluentAssertions;

    using Treatment.Console.Console;
    using Treatment.Console.Tests.E2ETests.Fakes;
    using Treatment.Core.Interfaces;

    using Xunit;

    public class ProgramTests
    {
        private readonly FakeBootstrapper _bootstrapper;
        private readonly FakeConsoleAdapter _console;
        private StringBuilder _sb;

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
            var result = Program.Main("");

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
            _console.ToString().Should().Be("Installed search providers (ordered by priority):\r\n- FileSystem\r\n");
        }

        [Fact]
        public void ListProvidersHelpShouldDisplayHelpTest()
        {
            // arrange

            // act
            var result = Program.Main("help list-providers");

            // assert
            result.Should().Be(-1);
            // _console.ToString().Should().Be(string.Empty);
            _sb.ToString().Should().Be("CommandLine 2.2.1\nCopyright (c) 2005 - 2018 Giacomo Stelluti Scala & Contributors\nERROR(S):\nVerb \'list-providers --help\' is not recognized.\n\n  --help       Display this help screen.\n\n  --version    Display version information.\n\n");
        }
    }
}