namespace Treatment.Core.Tests
{
    using System;

    using FluentAssertions;

    using SimpleInjector;

    using Xunit;

    public class CoreBootstrapTest
    {
        [Fact]
        public void Bootstrap_ResultsInValidContainerTest()
        {
            // arrange
            var container = new Container();
            CoreBootstrap.Bootstrap(container);

            // act
            Action act = () => container.Verify();

            // assert
            act.Should().NotThrow();

        }
    }
}