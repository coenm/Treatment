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
        }

        [Fact]
        public async Task StartSut()
        {
            var started = await fixture.Agent.StartSutAsync();
            started.Should().BeTrue();
        }
    }
}
