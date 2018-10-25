namespace Treatment.Console.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FluentAssertions;

    using SimpleInjector;

    using Treatment.Console.Bootstrap;
    using Treatment.Contract.DTOs;
    using Treatment.Contract.Queries;

    using Xunit;

    public class BootstrapperTest
    {
        [Fact]
        public void Bootstrap_ResultsInValidContainerTest()
        {
            // arrange
            var bootstrapper = new Bootstrapper();
            bootstrapper.Init();

            // act
            Action act = () => bootstrapper.VerifyContainer();

            // assert
            act.Should().NotThrow();
        }

        [Fact]
        public async Task GetAllSearchProvidersQuery_ShouldReturnOnlyFileSystem_WhenEverythingPluginIsNotReferencedTest()
        {
            // This test only succeeds when Everything plugin project is not referenced!
            // Or when Everything is not installed.
            // At this moment, we know this is the case. This test doesn't arrange this specifically

            // arrange
            var bootstrapper = new Bootstrapper();
            bootstrapper.Init();

            bootstrapper.RegisterDefaultOptions();
            bootstrapper.Container.Register(typeof(IHoldOnExitOption), () => new StaticOptions(VerboseLevel.Disabled, false, true, string.Empty, string.Empty), Lifestyle.Scoped);
            bootstrapper.VerifyContainer();

            using (bootstrapper.StartSession())
            {
                // act
                var result = await bootstrapper.ExecuteQueryAsync(GetAllSearchProvidersQuery.Instance);

                // assert
                result.Should().BeEquivalentTo(new List<SearchProviderInfo>(1)
                                                   {
                                                       new SearchProviderInfo(int.MaxValue, "FileSystem"),
                                                   });
            }
        }
    }
}
