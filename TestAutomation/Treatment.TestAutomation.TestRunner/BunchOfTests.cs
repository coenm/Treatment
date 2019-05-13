namespace Treatment.TestAutomation.TestRunner
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;
    using Treatment.TestAutomation.TestRunner.Framework;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;
    using Treatment.TestAutomation.TestRunner.XUnit;
    using Xunit;
    using Xunit.Abstractions;

    [Collection(nameof(TestFramework))]
    public class BunchOfTests
    {
        private readonly ITestOutputHelper output;
        private TestFrameworkFixture fixture;

        public BunchOfTests(TestFrameworkFixture fixture, ITestOutputHelper output)
        {
            this.output = output;
            this.fixture = fixture;

            Mouse = fixture.Mouse;
            Keyboard = fixture.Keyboard;
            Agent = fixture.Agent;
            Application = fixture.Application;

            output.WriteLine("ctor");
        }

        private IMouse Mouse { get; }

        private IKeyboard Keyboard { get; }

        private ITreatmentApplication Application { get; }

        private ITestAgent Agent { get; }

        [Fact]
        public async Task ReadTreatmentConfigFile()
        {
            var started = await Agent.StartSutAsync();
            started.Should().BeTrue();

            var sutExe = await Agent.LocateSutExecutableAsync();

            var directoryInfo = new FileInfo(sutExe).Directory;
            directoryInfo.Should().NotBeNull();

            var dir = directoryInfo.FullName;
            var content = await Agent.GetFileContentAsync(Path.Combine(dir, "TreatmentConfig.json"));

            content.Should().NotBeNull();
        }

        [Fact]
        public async Task StartSutAndCheckApplicationCreatedSetting()
        {
            var mre = new ManualResetEvent(false);
            Application.Startup += (_, __) => mre.Set();

            output.WriteLine("Start sut..");
            var started = await Agent.StartSutAsync();
            started.Should().BeTrue();

            mre.WaitOne(5000);
            Application.Created.Should().BeTrue();
        }

        [Fact]
        public async Task StartSut()
        {
            var started = await Agent.StartSutAsync();
            started.Should().BeTrue();

            await Mouse.DragAsync(40, 230, 100, 600);

            await Task.Delay(10000);
            await Mouse.MoveCursorAsync(200, 100);

            await Task.Delay(1000);
            await Mouse.MoveCursorAsync(40, 30);

            await Task.Delay(1000);
            await Mouse.ClickAsync();

            Application.Created.Should().BeTrue();
        }
    }
}
