﻿namespace Treatment.Console.Tests
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using SimpleInjector;

    using Treatment.Console.Bootstrap;
    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Contract.DTOs;
    using Treatment.Contract.Queries;

    using Xunit;

    public class Bootstrapper2Test
    {
        [Fact]
        public void Bootstrap_ResultsInValidContainerTest()
        {
            // arrange
            var bootstrapper = new Bootstrapper2();
            bootstrapper.Init();

            // act
            Action act = () => bootstrapper.VerifyContainer();

            // assert
            act.Should().NotThrow();
        }


        [Fact]
        public void Abcdef()
        {
            var bootstrapper = new Bootstrapper2();
            bootstrapper.Init();

            bootstrapper.RegisterDefaultOptions();
            bootstrapper.Container.Register(typeof(IHoldOnExitOption), () => new StaticOptions(VerboseLevel.Null, false, true, string.Empty), Lifestyle.Scoped);
            bootstrapper.VerifyContainer();

            using (bootstrapper.StartSession())
            {
                var result = bootstrapper.ExecuteQuery(new GetAllSearchProvidersQuery());

                result.Should().BeEquivalentTo(new List<SearchProviderInfo>(1)
                                                   {
                                                       new SearchProviderInfo(int.MaxValue, "FileSystem")
                                                   });
            }
        }

        [Fact]
        public void Abcdefaa()
        {
            var bootstrapper = new Bootstrapper2();
            bootstrapper.Init();

            bootstrapper.RegisterDefaultOptions();
            // bootstrapper.Container.Register(typeof(IHoldOnExitOption), () => new StaticOptions(VerboseLevel.Null, false, true, string.Empty), Lifestyle.Scoped);
            // bootstrapper.VerifyContainer();

            using (bootstrapper.StartSession())
            {
                var commandHandler = bootstrapper.Container.GetInstance<ICommandHandler<UpdateProjectFilesCommand>>();
                commandHandler.Execute(new UpdateProjectFilesCommand(@"D:\tmp\aAP"));

            }
        }
    }
}