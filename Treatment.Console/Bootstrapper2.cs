namespace Treatment.Console
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using SimpleInjector;
    using SimpleInjector.Lifestyles;

    using Treatment.Console.Console;
    using Treatment.Console.CrossCuttingConcerns;
    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Core;
    using Treatment.Core.Interfaces;

    public static class Bootstrapper2
    {
        private static Container _container;

        public static object GetCommandHandler(Type commandType) =>
            _container.GetInstance(typeof(ICommandHandler<>).MakeGenericType(commandType));

        public static object GetQueryHandler(Type queryType) =>
            _container.GetInstance(CreateQueryHandlerType(queryType));

        public static TResult ExecuteQuery<TResult>(IQuery<TResult> query, Container container)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = container.GetInstance(handlerType);

            return handler.Handle((dynamic)query);
        }

        public static IEnumerable<Type> GetCommandTypes() => CoreBootstrap.GetCommandTypes();

        public static IEnumerable<Type> GetQueryAndResultTypes()
        {
            var queryTypes = CoreBootstrap.GetQueryTypes().Select(q => q.QueryType);
            var resultTypes = CoreBootstrap.GetQueryTypes().Select(q => q.ResultType).Distinct();
            return queryTypes.Concat(resultTypes);
        }

        public static IDisposable StartSession(Container container)
        {
            return AsyncScopedLifestyle.BeginScope(container);
        }

        public static void Bootstrap(Container container = null)
        {
            _container = container;
            if (container == null)
                _container = new Container();

            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            CoreBootstrap.Bootstrap(_container);

            _container.RegisterInstance<IConsole>(ConsoleAdapter.Instance);

            _container.RegisterDecorator(
                                        typeof(ICommandHandler<>),
                                        typeof(SetRootDirectoryCommandHandlerDecorator<>),
                                        Lifestyle.Scoped,
                                        context => typeof(IDirectoryProperty).IsAssignableFrom(context.ServiceType.GetGenericArguments()[0]));

            //
            // _container.RegisterDecorator(
            //                              typeof(ICommandHandler<>),
            //                              typeof(SetFileSearchSelectorCommandHandlerDecorator<>),
            //                              Lifestyle.Scoped,
            //                              context => typeof(IDirectoryProperty).IsAssignableFrom(context.ServiceType.GetGenericArguments()[0]));
            //



            _container.Register<IRootDirSanitizer, RemoveRootDirSanitizer>(Lifestyle.Scoped);

            // todo register the rest outside core

            _container.Verify();
        }

        private static Type CreateQueryHandlerType(Type queryType) =>
            typeof(IQueryHandler<,>).MakeGenericType(queryType, new QueryInfo(queryType).ResultType);
    }
}