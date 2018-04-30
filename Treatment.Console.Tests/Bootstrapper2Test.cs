namespace Treatment.Console.Tests
{
    using System;
    using System.Linq;

    using FluentAssertions;

    using SimpleInjector;

    using Treatment.Contract;
    using Treatment.Contract.Queries;

    using Xunit;

    public class Bootstrapper2Test
    {
        [Fact]
        public void Bootstrap_ResultsInValidContainerTest()
        {
            // arrange
            var container = new Container();
            Bootstrapper2.Bootstrap(container);

            // act
            Action act = () => container.Verify();

            // assert
            act.Should().NotThrow();
        }


        [Fact]
        public void Abcdef()
        {
            var container = new Container();
            Bootstrapper2.Bootstrap(container);

            using (Bootstrapper2.StartSession(container))
            {
                var x = Bootstrapper2.GetCommandTypes();
                var x1 = Bootstrapper2.GetQueryAndResultTypes();
                var x2 = Bootstrapper2.GetQueryHandler(x1.First());


                var result = ExecuteQuery(new GetAllSearchProvidersQuery(), container);

                result.Should().NotBeNull();
            }

        }

        public TResult ExecuteQuery<TResult>(IQuery<TResult> query, Container container)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = container.GetInstance(handlerType);

            return handler.Handle((dynamic)query);
        }
    }
}