namespace Treatment.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using FluentValidation;

    using JetBrains.Annotations;

    using SimpleInjector;
    using SimpleInjector.Lifestyles;

    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Core.DefaultPluginImplementation;
    using Treatment.Core.DefaultPluginImplementation.FileSearch;
    using Treatment.Core.FileSystem;
    using Treatment.Core.Interfaces;
    using Treatment.Core.UseCases.CrossCuttingConcerns;

    // This class allows registering all types that are defined in the business layer, and are shared across
    // all applications that use this layer (WCF and Web API). For simplicity, this class is placed inside
    // this assembly, but this does couple the business layer assembly to the used container. If this is a
    // concern, create a specific BusinessLayer.Bootstrap project with this class.
    public class CoreBootstrap
    {
        private static readonly Assembly[] _contractAssemblies = { typeof(IQuery<>).Assembly };
        private static readonly Assembly[] _businessLayerAssemblies = { Assembly.GetExecutingAssembly() };

        public static void Bootstrap([NotNull] Container container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            if (container.Options.DefaultScopedLifestyle == null)
                container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            // container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            RegisterFluentValidationValidators(container);

            container.Register(typeof(ICommandHandler<>), _businessLayerAssemblies, Lifestyle.Scoped);

            RegisterCommandValidationCommandHandlerDecoraters(container);


            // container.RegisterDecorator(typeof(ICommandHandler<>), typeof(AuthorizationCommandHandlerDecorator<>));

            container.Register(typeof(IQueryHandler<,>), _businessLayerAssemblies, Lifestyle.Scoped);
            // container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(AuthorizationQueryHandlerDecorator<,>));


            container.RegisterInstance<IFileSystem>(OsFileSystem.Instance);

            // Plugins might register more Search Provider Factories
            container.RegisterCollection<ISearchProviderFactory>(new[] { typeof(OsFileSystemSearchProviderFactory) });

            // container.RegisterSingleton<FileSearchSelector>();
            // container.RegisterSingleton(() => container.GetInstance<FileSearchSelector>().CreateSearchProvider());

            container.Register<IFileSearchSelector, FileSearchSelector>(Lifestyle.Scoped);
            container.Register(() => container.GetInstance<IFileSearchSelector>().CreateSearchProvider(), Lifestyle.Scoped);


        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RegisterCommandValidationCommandHandlerDecoraters([NotNull] Container container)
        {
            container.RegisterDecorator(
                                        typeof(ICommandHandler<>),
                                        typeof(CommandValidationDecorator<>),
                                        Lifestyle.Scoped);
        }

        private static void RegisterFluentValidationValidators(Container container)
        {
            // var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            container.Register(typeof(IValidator<>), _businessLayerAssemblies, Lifestyle.Scoped);
        }

        // not okeay
        public static IEnumerable<Type> GetCommandTypes() =>
            from assembly in _contractAssemblies
            from type in assembly.GetExportedTypes()
            where type.Name.EndsWith("Command")
            select type;

        public static IEnumerable<QueryInfo> GetQueryTypes() =>
            from assembly in _contractAssemblies
            from type in assembly.GetExportedTypes()
            where QueryInfo.IsQuery(type)
            select new QueryInfo(type);
    }

    [DebuggerDisplay("{QueryType.Name,nq}")]
    public sealed class QueryInfo
    {
        public readonly Type QueryType;
        public readonly Type ResultType;

        public QueryInfo(Type queryType)
        {
            QueryType = queryType;
            ResultType = DetermineResultTypes(queryType).Single();
        }

        public static bool IsQuery(Type type) => DetermineResultTypes(type).Any();

        private static IEnumerable<Type> DetermineResultTypes(Type type) =>
            from interfaceType in type.GetInterfaces()
            where interfaceType.IsGenericType
            where interfaceType.GetGenericTypeDefinition() == typeof(IQuery<>)
            select interfaceType.GetGenericArguments()[0];
    }
}