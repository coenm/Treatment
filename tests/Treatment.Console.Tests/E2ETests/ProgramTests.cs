namespace Treatment.Console.Tests.E2ETests
{
    using System;
    using System.IO;
    using System.Text;

    using ApprovalTests;

    using FluentAssertions;

    using JetBrains.Annotations;

    using Treatment.Console.Console;
    using Treatment.Console.Tests.E2ETests.Fakes;

    using Xunit;

    public class ProgramTests
    {
        private readonly FakeBootstrapper bootstrapper;
        private readonly StringBuilder sb;

        [NotNull]
        private readonly FakeConsoleAdapter console;

        public ProgramTests()
        {
            console = new FakeConsoleAdapter();

            bootstrapper = new FakeBootstrapper();
            bootstrapper.RegisterPostRegisterAction(container => container.RegisterInstance<IConsole>(console));
            // _bootstrapper.RegisterPostRegisterAction(container => container.RegisterInstance<IFileSystem>(_console));
            // container.RegisterInstance<IFileSystem>(OsFileSystem.Instance);

            sb = new StringBuilder();
            Console.SetOut(new StringWriter(sb));
            Console.SetError(new StringWriter(sb));

            Program.Bootstrapper = bootstrapper;
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
            Approvals.Verify(console.ToString());
        }
    }
}
