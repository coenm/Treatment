namespace Treatment.TestAutomation.TestRunner
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;
    using JetBrains.Annotations;
    using TestAgent.Contract.Interface.Input.Enums;
    using TestHelper;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Application;
    using Treatment.TestAutomation.TestRunner.Controls.Framework;
    using Treatment.TestAutomation.TestRunner.Controls.Interfaces;
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
        public async Task RepeatTest()
        {
            for (int i = 0; i < 5; i++)
            {
                await StartSutAndCheckApplicationCreatedSetting();
                await Task.Delay(40);
            }
        }

        [Fact]
        public async Task StartSutAndCheckApplicationCreatedSetting()
        {
            var mre = new ManualResetEvent(false);
            var mre2 = new AutoResetEvent(false);

            fixture.ApplicationAvailable += (_, __) => mre.Set();

            output.WriteLine("Start sut..");
            var started = await Agent.StartSutAsync();
            started.Should().BeTrue();

            mre.WaitOne(15000).Should().BeTrue("Application not started in time");
            // Application.Created.Should().BeTrue();

            Application.WindowActivated += (_, __) => mre2.Set();
            mre2.WaitOne(10000).Should().BeTrue("No Window activated in time");

            for (var j = 0; j < 5; j++)
            {
                // var status = Application.MainWindow.StatusBar.StatusConfigFilename as RemoteTextBlock;
                var openSettingsButton = Application.MainWindow.OpenSettingsButton as RemoteButton;
                openSettingsButton.Should().NotBeNull();

                await Mouse.MoveMouseCursorToElementAsync(openSettingsButton);
                await Mouse.ClickAsync();
                mre2.WaitOne(1000);
                Application.SettingsWindow.Should().NotBeNull("Application window should not be null");

                await Task.Delay(50);

                var checkbox = Application.SettingsWindow.DelayExecution as RemoteCheckBox;
                checkbox.Should().NotBeNull("Checkbox DelayExecution expected to be there.");
                output.WriteLine($"checkbox.IsChecked: {checkbox.IsChecked}");

                await Mouse.MoveMouseCursorToElementAsync(checkbox);
                await Task.Delay(1000);
                await Mouse.ClickAsync();
                await Task.Delay(1000);
                output.WriteLine($"checkbox.IsChecked: {checkbox.IsChecked}");
                await Task.Delay(1000);

                var combo = Application.SettingsWindow.ComboSearchProvider as RemoteComboBox;
                combo.Should().NotBeNull();

                for (int i = 0; i < 3; i++)
                {
                    await Mouse.MoveMouseCursorToElementAsync(combo);
                    await Mouse.ClickAsync();
                    await Keyboard.PressAsync(VirtualKeyCode.Down);
                    await Keyboard.PressAsync(VirtualKeyCode.Up);
                    await Keyboard.PressAsync(VirtualKeyCode.Escape);
                }

                await Keyboard.PressAsync(VirtualKeyCode.Escape);
                mre2.WaitOne(1000);

                var window = Application.MainWindow as RemoteMainWindow;
                window.Should().NotBeNull();
                output.WriteLine($"x {window.Position.X}  y {window.Position.Y}");
                output.WriteLine($"x {window.Size.Width}  y {window.Size.Height}");

                var x = (int)(window.Position.X + window.Size.Width - 50);
                var y = (int)(window.Position.Y - 10);
                output.WriteLine($"x {x}  y {y}");
                await Mouse.MoveCursorAsync(x, y);
            }

            var window1 = Application.MainWindow as RemoteMainWindow;
            window1.Should().NotBeNull();
            var x1 = (int)(window1.Position.X + window1.Size.Width - 250);
            var y1 = (int)(window1.Position.Y - 10);
            await Mouse.MoveCursorAsync(x1, y1);
            await Mouse.MouseDownAsync();

            for (var i = 0; i < 250; i++)
            {
                x1++;
                y1++;
                await Mouse.MoveCursorAsync(x1, y1);
            }

            for (var i = 0; i < 250; i += 2)
            {
                x1 -= 2;
                y1 -= 2;
                await Mouse.MoveCursorAsync(x1, y1);
            }

            for (var i = 0; i < 250; i += 3)
            {
                x1 += 3;
                y1 += 3;
                await Mouse.MoveCursorAsync(x1, y1);
            }

            for (var i = 0; i < 250; i += 4)
            {
                x1 -= 4;
                y1 -= 4;
                await Mouse.MoveCursorAsync(x1, y1);
            }

            for (var i = 0; i < 250; i += 5)
            {
                x1 += 5;
                y1 += 5;
                await Mouse.MoveCursorAsync(x1, y1);
            }

            for (var i = 0; i < 250; i += 6)
            {
                x1 -= 6;
                y1 -= 6;
                await Mouse.MoveCursorAsync(x1, y1);
            }

            await Mouse.MouseUpAsync();

            // give events time to pass. Sometimes, the window has been blown to full screen.
            await Task.Delay(500);

            x1 = (int)(window1.Position.X + window1.Size.Width - 50);
            y1 = (int)(window1.Position.Y - 10);
            await Mouse.MoveCursorAsync(x1, y1);

            var mre3 = new ManualResetEvent(false);
            void ApplicationOnExit(object sender, ApplicationExit e)
            {
                mre3.Set();
            }

            Application.Exit += ApplicationOnExit;
            await Mouse.ClickAsync();
            if (!mre3.WaitOne(1000))
            {
                output.WriteLine("Try to close the application with the alt f4 keys.");
                await Keyboard.KeyCombinationPressAsync(VirtualKeyCode.Alt, VirtualKeyCode.F4);
            }

            if (!mre3.WaitOne(1000))
            {
                output.WriteLine("Try to close the application with the alt f4 keys.");
                await Keyboard.KeyCombinationPressAsync(VirtualKeyCode.Alt, VirtualKeyCode.F4);
            }

            mre3.WaitOne(1000).Should().BeTrue();
            Application.Exit -= ApplicationOnExit;
        }

        [ConditionalHostFact(TestHostMode.Skip, TestHost.AppVeyor)]
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
