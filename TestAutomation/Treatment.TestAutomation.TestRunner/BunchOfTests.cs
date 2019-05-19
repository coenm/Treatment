namespace Treatment.TestAutomation.TestRunner
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;
    using global::TestAutomation.Input.Contract.Interface.Input.Enums;
    using JetBrains.Annotations;

    using Treatment.TestAutomation.TestRunner.Controls.Framework;
    using Treatment.TestAutomation.TestRunner.Framework;
    using Treatment.TestAutomation.TestRunner.Framework.Interfaces;
    using Treatment.TestAutomation.TestRunner.XUnit;
    using Xunit;
    using Xunit.Abstractions;

    [Collection(nameof(TestFramework))]
    public class BunchOfTests
    {
        [NotNull] private readonly ITestOutputHelper output;
        [NotNull] private readonly TestFrameworkFixture fixture;

        public BunchOfTests(TestFrameworkFixture fixture, ITestOutputHelper output)
        {
            this.output = output;
            this.fixture = fixture;

            output.WriteLine("ctor");
        }

        private IMouse Mouse => fixture.Mouse;

        private IKeyboard Keyboard => fixture.Keyboard;

        private ITreatmentApplication Application => fixture.Application;

        private ITestAgent Agent => fixture.Agent;

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
            fixture.ApplicationAvailable += (_, __) => mre.Set();

            output.WriteLine("Start sut..");
            var started = await Agent.StartSutAsync();
            started.Should().BeTrue();

            mre.WaitOne(15000);
//            Application.Created.Should().BeTrue();

            await Task.Delay(10000);

            // var status = Application.MainWindow.StatusBar.StatusConfigFilename as RemoteTextBlock;
            var status = Application.MainWindow.OpenSettingsButton as RemoteButton;

            output.WriteLine($"x {status.Position.X}  y {status.Position.Y}");
            output.WriteLine($"x {status.Size.Width}  y {status.Size.Height}");
            var x = (int)(status.Position.X + (status.Size.Width / 2));
            var y = (int)(status.Position.Y + (status.Size.Height / 2));
            output.WriteLine($"x {x}  y {y}");

            await Mouse.MoveCursorAsync(x, y);

            await Task.Delay(1000);

            await Mouse.ClickAsync();

            await Task.Delay(6000);

            Application.SettingsWindow.Should().NotBeNull();

            await Keyboard.PressAsync(VirtualKeyCode.Escape);

            await Task.Delay(6000);

            output.WriteLine("moved mouse");
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
