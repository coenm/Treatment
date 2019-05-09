namespace Treatment.TestAutomation.TestRunner
{
    using System.Threading.Tasks;

    using FluentAssertions;
    using Treatment.TestAutomation.TestRunner.Sut;
    using Xunit;
    using Xunit.Abstractions;

    [Collection(nameof(StartedTreatment))]
    public class BunchOfTests
    {
        private readonly StartedTreatmentFixture fixture;
        private readonly ITestOutputHelper output;

        public BunchOfTests(StartedTreatmentFixture fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            this.output = output;

            Mouse = fixture.Mouse;
            Keyboard = fixture.Keyboard;
        }

        private IMouse Mouse { get; }

        private IKeyboard Keyboard { get; }

        [Fact]
        public async Task StartSut()
        {
            var started = await fixture.Agent.StartSutAsync();
            started.Should().BeTrue();

            await Task.Delay(1000);
            await Mouse.DragAsync(40, 230, 100, 600);

            await Task.Delay(10000);
            await Mouse.MoveCursorAsync(200, 100);

            await Task.Delay(1000);
            await Mouse.MoveCursorAsync(40, 30);

            await Task.Delay(1000);
            await Mouse.ClickAsync();
        }
    }
}
