namespace Treatment.TestAutomation.TestRunner
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;
    using JetBrains.Annotations;
    using TestAgent.Contract.Interface.Input.Enums;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Application;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
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
        public async Task SettingsWindow_ShouldSaveConfig_WhenConfigIsChangedAndOkButtonIsClicked()
        {
            const string configFilename = "TreatmentConfig.json";

            var mre = new ManualResetEvent(false);
            var mre2 = new AutoResetEvent(false);

            fixture.ApplicationAvailable += (_, __) => mre.Set();

            var started = await Agent.StartSutAsync();
            started.Should().BeTrue();

            mre.WaitOne(15000).Should().BeTrue("Application not started in time");

            Application.WindowActivated += (_, __) => mre2.Set();
            mre2.WaitOne(10000).Should().BeTrue("No Window activated in time");

            // remove config file when exists
            var deleted = await Agent.DeleteFileAsync(configFilename);
            deleted.Should().BeTrue();

            // open settings window.
            var settingsWindow = await OpenSettingsWindowAsync(mre2);
            await ConfigurableDelay(50);

            var checkbox = settingsWindow.DelayExecution as RemoteCheckBox;
            var delayExecutionMinTextBox = settingsWindow.DelayExecutionMinValue as RemoteTextBox;
            var delayExecutionMaxTextBox = settingsWindow.DelayExecutionMaxValue as RemoteTextBox;
            checkbox.Should().NotBeNull("Checkbox DelayExecution expected to be there.");
            delayExecutionMinTextBox.Should().NotBeNull("TextBox DelayExecutionMinTextBox expected to be there.");
            delayExecutionMaxTextBox.Should().NotBeNull("TextBox DelayExecutionMaxTextBox expected to be there.");

            await Mouse.MoveMouseCursorToElementAsync(checkbox);
            await Mouse.ClickAsync();
            await Mouse.ClickAsync();
            await Task.Delay(100);

            await CheckCheckboxAsync(checkbox);
            await Task.Delay(100);
            delayExecutionMinTextBox.IsEnabled.Should().BeTrue();
            delayExecutionMaxTextBox.IsEnabled.Should().BeTrue();
            await ConfigurableDelay(100);

            await UnCheckCheckboxAsync(checkbox);
            await Task.Delay(100);
            delayExecutionMinTextBox.IsEnabled.Should().BeFalse();
            delayExecutionMaxTextBox.IsEnabled.Should().BeFalse();
            await ConfigurableDelay(100);

            await CheckCheckboxAsync(checkbox);
            await Task.Delay(100);
            delayExecutionMinTextBox.IsEnabled.Should().BeTrue();
            delayExecutionMaxTextBox.IsEnabled.Should().BeTrue();
            await ConfigurableDelay(100);

            await SetTextAsync(delayExecutionMaxTextBox, "420");
            await ConfigurableDelay(100);

            await SetTextAsync(delayExecutionMinTextBox, "160");
            await ConfigurableDelay(100);

            // accept changes
            var okButton = settingsWindow.OkButton as RemoteButton;
            okButton.Should().NotBeNull("Button should be there on ui");
            await Mouse.MoveMouseCursorToElementAsync(okButton);
            await Mouse.ClickAsync();

            mre2.WaitOne(1000);

            // check if file has been saved.
            await ConfigurableDelay(100, "wait for file to be saved.");
            var fileExists = await Agent.FileExistsAsync(configFilename);
            fileExists.Should().BeTrue("file should be saved already");

            var content = await Agent.GetFileContentAsync(configFilename);
            content.Should().NotBeNull("config file should have content");
            output.WriteLine(Encoding.Default.GetString(content));

            // close application.
            var window = Application.MainWindow as RemoteMainWindow;
            window.Should().NotBeNull();
            output.WriteLine($"x {window.Position.X}  y {window.Position.Y}");
            output.WriteLine($"x {window.Size.Width}  y {window.Size.Height}");

            var x = (int)(window.Position.X + window.Size.Width - 50);
            var y = (int)(window.Position.Y - 10);
            output.WriteLine($"x {x}  y {y}");
            await Mouse.MoveCursorAsync(x, y);

            var window1 = Application.MainWindow as RemoteMainWindow;
            window1.Should().NotBeNull();
            var x1 = (int)(window1.Position.X + window1.Size.Width - 250);
            var y1 = (int)(window1.Position.Y - 10);
            await Mouse.MoveCursorAsync(x1, y1);

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

        [Fact]
        public async Task SettingsWindow_ShouldAllowInputDelayMinAndMax_WhenCheckboxIsChecked()
        {
            var mre = new ManualResetEvent(false);
            var mre2 = new AutoResetEvent(false);

            fixture.ApplicationAvailable += (_, __) => mre.Set();

            var started = await Agent.StartSutAsync();
            started.Should().BeTrue();

            mre.WaitOne(15000).Should().BeTrue("Application not started in time");

            Application.WindowActivated += (_, __) => mre2.Set();
            mre2.WaitOne(10000).Should().BeTrue("No Window activated in time");

            // open settings window.
            var settingsWindow = await OpenSettingsWindowAsync(mre2);
            await ConfigurableDelay(50);

            var checkbox = settingsWindow.DelayExecution as RemoteCheckBox;
            var delayExecutionMinTextBox = settingsWindow.DelayExecutionMinValue as RemoteTextBox;
            var delayExecutionMaxTextBox = settingsWindow.DelayExecutionMaxValue as RemoteTextBox;
            checkbox.Should().NotBeNull("Checkbox DelayExecution expected to be there.");
            delayExecutionMinTextBox.Should().NotBeNull("TextBox DelayExecutionMinTextBox expected to be there.");
            delayExecutionMaxTextBox.Should().NotBeNull("TextBox DelayExecutionMaxTextBox expected to be there.");

            await Mouse.MoveMouseCursorToElementAsync(checkbox);
            await Mouse.ClickAsync();
            await Mouse.ClickAsync();
            await Task.Delay(100);

            await CheckCheckboxAsync(checkbox);
            await Task.Delay(100);
            delayExecutionMinTextBox.IsEnabled.Should().BeTrue();
            delayExecutionMaxTextBox.IsEnabled.Should().BeTrue();
            await ConfigurableDelay(100);

            await UnCheckCheckboxAsync(checkbox);
            await Task.Delay(100);
            delayExecutionMinTextBox.IsEnabled.Should().BeFalse();
            delayExecutionMaxTextBox.IsEnabled.Should().BeFalse();
            await ConfigurableDelay(100);

            await CheckCheckboxAsync(checkbox);
            await Task.Delay(100);
            delayExecutionMinTextBox.IsEnabled.Should().BeTrue();
            delayExecutionMaxTextBox.IsEnabled.Should().BeTrue();
            await ConfigurableDelay(100);

            await SetTextAsync(delayExecutionMaxTextBox, "420");
            await ConfigurableDelay(100);

            await SetTextAsync(delayExecutionMinTextBox, "160");
            await ConfigurableDelay(100);

            // accept changes
            var okButton = settingsWindow.OkButton as RemoteButton;
            okButton.Should().NotBeNull("Button should be there on ui");
            await Mouse.MoveMouseCursorToElementAsync(okButton);
            await Mouse.ClickAsync();

            mre2.WaitOne(1000);
            settingsWindow = null;

            var window = Application.MainWindow as RemoteMainWindow;
            window.Should().NotBeNull();
            output.WriteLine($"x {window.Position.X}  y {window.Position.Y}");
            output.WriteLine($"x {window.Size.Width}  y {window.Size.Height}");

            var x = (int)(window.Position.X + window.Size.Width - 50);
            var y = (int)(window.Position.Y - 10);
            output.WriteLine($"x {x}  y {y}");
            await Mouse.MoveCursorAsync(x, y);

            var window1 = Application.MainWindow as RemoteMainWindow;
            window1.Should().NotBeNull();
            var x1 = (int)(window1.Position.X + window1.Size.Width - 250);
            var y1 = (int)(window1.Position.Y - 10);
            await Mouse.MoveCursorAsync(x1, y1);

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

        [Fact]
        public async Task RepeatTest()
        {
            for (int i = 0; i < 20; i++)
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
                await OpenSettingsWindowAsync(mre2);
                await ConfigurableDelay(50);

                var checkbox = Application.SettingsWindow.DelayExecution as RemoteCheckBox;
                checkbox.Should().NotBeNull("Checkbox DelayExecution expected to be there.");
                output.WriteLine($"checkbox.IsChecked: {checkbox.IsChecked}");

                await Mouse.MoveMouseCursorToElementAsync(checkbox);
                await Mouse.ClickAsync();
                output.WriteLine($"checkbox.IsChecked: {checkbox.IsChecked}");
                await ConfigurableDelay(100);

                var combo = Application.SettingsWindow.ComboSearchProvider as RemoteComboBox;
                combo.Should().NotBeNull();

                for (var i = 0; i < 3; i++)
                {
                    await Mouse.MoveMouseCursorToElementAsync(combo);
                    await Mouse.ClickAsync();
                    await Keyboard.PressAsync(VirtualKeyCode.Down);
                    await Keyboard.PressAsync(VirtualKeyCode.Up);
                    await Keyboard.PressAsync(VirtualKeyCode.Escape);

                    await Keyboard.PressAsync(VirtualKeyCode.Tab);
                    await Keyboard.PressAsync(VirtualKeyCode.Tab);
                    await Keyboard.PressAsync(VirtualKeyCode.Tab);
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

        private Task ConfigurableDelay(int milliseconds, string reason = "no reason given")
        {
            if (milliseconds <= 0)
                return Task.CompletedTask;

            // wraps delay so we can make this configurable..
            output.WriteLine($"Waiting.. {TimeSpan.FromMilliseconds(milliseconds)} because {reason}.");
            return Task.Delay(milliseconds);
        }

        private async Task CheckCheckboxAsync([NotNull] RemoteCheckBox checkbox)
        {
            if (checkbox.IsChecked.HasValue && checkbox.IsChecked.Value)
                return;

            var mre = new ManualResetEvent(false);
            void CheckboxOnOnChecked(object sender, OnChecked e) => mre.Set();

            checkbox.OnChecked += CheckboxOnOnChecked;
            try
            {
                await Mouse.MoveMouseCursorToElementAsync(checkbox);
                await Mouse.ClickAsync();
                if (!mre.WaitOne(1000))
                {
                    await Mouse.ClickAsync();
                    mre.WaitOne(1000);
                }
            }
            finally
            {
                checkbox.OnChecked -= CheckboxOnOnChecked;
            }

            output.WriteLine($"checkbox.IsChecked: {checkbox.IsChecked}");
            checkbox.IsChecked.Should().BeTrue($"Mouse click should have checked the checkbox.");
        }

        private async Task UnCheckCheckboxAsync([NotNull] RemoteCheckBox checkbox)
        {
            if (checkbox.IsChecked.HasValue && checkbox.IsChecked.Value == false)
                return;

            var mre = new ManualResetEvent(false);
            void CheckboxOnUnChecked(object sender, OnUnChecked e) => mre.Set();

            checkbox.OnUnChecked += CheckboxOnUnChecked;
            try
            {
                await Mouse.MoveMouseCursorToElementAsync(checkbox);
                await Mouse.ClickAsync();
                if (!mre.WaitOne(1000))
                {
                    await Mouse.ClickAsync();
                    mre.WaitOne(1000);
                }
            }
            finally
            {
                checkbox.OnUnChecked -= CheckboxOnUnChecked;
            }

            output.WriteLine($"checkbox.IsChecked: {checkbox.IsChecked}");
            checkbox.IsChecked.Should().BeFalse($"Mouse click should have unchecked the checkbox.");
        }

        private async Task SetTextAsync([NotNull] RemoteTextBox textbox, string text)
        {
            textbox.IsEnabled.Should().BeTrue($"we want to change the text");

            var mre = new ManualResetEvent(false);

            void TextboxOnGotFocus(object sender, GotFocus e) => mre.Set();

            if (!textbox.HasFocus)
            {
                textbox.GotFocus += TextboxOnGotFocus;

                try
                {
                    await Mouse.MoveMouseCursorToElementAsync(textbox);
                    await Mouse.ClickAsync();
                    mre.WaitOne(1000);
                    textbox.HasFocus.Should().BeTrue($"we want to change the text and focus is required");
                }
                finally
                {
                    textbox.GotFocus += TextboxOnGotFocus;
                }

                await Mouse.MoveMouseCursorToElementAsync(textbox);
                await Mouse.ClickAsync();
            }

            mre.Reset();

            // remove everything that is inside..
            void TextboxOnTextValueChanged(object sender, TextValueChanged e) => mre.Set();
            textbox.TextValueChanged += TextboxOnTextValueChanged;

            try
            {
                if (string.IsNullOrEmpty(textbox.Value))
                    mre.Set();

                await Keyboard.KeyCombinationPressAsync(VirtualKeyCode.Control, VirtualKeyCode.KeyA);
                await Keyboard.KeyCombinationPressAsync(VirtualKeyCode.Delete);
                mre.WaitOne(1000);
                textbox.Value.Should().BeEmpty("we just removed all what was inside.");
            }
            finally
            {
                textbox.TextValueChanged -= TextboxOnTextValueChanged;
            }

            textbox.TextValueChanged += TextboxOnTextValueChanged;
            try
            {
                foreach (var c in text.ToCharArray())
                {
                    var cachedValue = textbox.Value;
                    mre.Reset();
                    await Keyboard.PressCharacterAsync(c);
                    mre.WaitOne(1000);
                    textbox.Value.Should().Be(cachedValue + c, $"this key was entered on the keyboard.");
                }
            }
            finally
            {
                textbox.TextValueChanged -= TextboxOnTextValueChanged;
            }
        }

        private async Task<RemoteSettingWindow> OpenSettingsWindowAsync(AutoResetEvent manualResetEventWindowCreated)
        {
            var openSettingsButton = Application.MainWindow.OpenSettingsButton as RemoteButton;
            openSettingsButton.Should().NotBeNull();

            // ReSharper disable once AssignNullToNotNullAttribute
            await Mouse.MoveMouseCursorToElementAsync(openSettingsButton);
            await Mouse.ClickAsync();

            manualResetEventWindowCreated.WaitOne(1000);

            await Task.Delay(100);

            var settingsWindow = Application.SettingsWindow;
            settingsWindow.Should().NotBeNull("Application window should not be null");

            return settingsWindow as RemoteSettingWindow;
        }
    }
}
