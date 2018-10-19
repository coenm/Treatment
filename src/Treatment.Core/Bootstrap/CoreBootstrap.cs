namespace Treatment.Core.Bootstrap
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
    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Contract.Plugin.SourceControl;
    using Treatment.Core.DefaultPluginImplementation.FileSearch;
    using Treatment.Core.DefaultPluginImplementation.SourceControl;
    using Treatment.Core.FileSystem;
    using Treatment.Core.Interfaces;
    using Treatment.Core.UseCases.CleanAppConfig;
    using Treatment.Core.UseCases.CrossCuttingConcerns;

    // This class allows registering all types that are defined in the business layer, and are shared across
    // all applications that use this layer (WCF and Web API). For simplicity, this class is placed inside
    // this assembly, but this does couple the business layer assembly to the used container. If this is a
    // concern, create a specific BusinessLayer.Bootstrap project with this class.
    public static class CoreBootstrap
    {
        private static readonly Assembly[] _contractAssemblies = { typeof(IQuery<>).Assembly };
        private static readonly Assembly[] _businessLayerAssemblies = { Assembly.GetExecutingAssembly() };

        public static void Bootstrap([NotNull] Container container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            if (container.Options.DefaultScopedLifestyle == null)
                container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            RegisterFluentValidationValidators(container);

            container.Register(typeof(ICommandHandler<>), _businessLayerAssemblies, Lifestyle.Scoped);

            RegisterCommandValidationCommandHandlerDecorators(container);

            // container.RegisterDecorator(typeof(ICommandHandler<>), typeof(AuthorizationCommandHandlerDecorator<>));

            container.Register(typeof(IQueryHandler<,>), _businessLayerAssemblies, Lifestyle.Scoped);
            // container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(AuthorizationQueryHandlerDecorator<,>));

            // container.Register<IFileSystem>(() => OsFileSystem.Instance, Lifestyle.Singleton);
            container.RegisterInstance<IFileSystem>(OsFileSystem.Instance);

            container.Collection.Register<ISearchProviderFactory>(new[] { typeof(OsFileSystemSearchProviderFactory) });
            container.Register<IFileSearchSelector, FileSearchSelector>(Lifestyle.Scoped);
            container.Register<IFileSearch>(() => container.GetInstance<IFileSearchSelector>().CreateSearchProvider(), Lifestyle.Scoped);
            container.Register<ISearchProviderNameOption, DefaultSearchProviderNameOption>(Lifestyle.Singleton);

            container.Collection.Register<ISourceControlAbstractFactory>(new[] { typeof(DummySourceControlFactory) });
            container.Register<ISourceControlSelector, SourceControlSelector>(Lifestyle.Scoped);
            container.Register<IReadOnlySourceControl>(() => container.GetInstance<ISourceControlSelector>().CreateSourceControl(), Lifestyle.Scoped);
            container.Register<ISourceControlNameOption, DefaultSourceControlNameOption>(Lifestyle.Singleton);

            container.Register<ICleanSingleAppConfig, CleanSingleAppConfig>(Lifestyle.Scoped); // is this correct?

            container.RegisterSingleton<IQueryProcessor, QueryProcessor>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RegisterCommandValidationCommandHandlerDecorators([NotNull] Container container)
        {
            container.RegisterDecorator(
                                        typeof(ICommandHandler<>),
                                        typeof(CommandValidationDecorator<>),
                                        Lifestyle.Scoped);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RegisterFluentValidationValidators(Container container)
        {
            // var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            container.Register(typeof(IValidator<>), _businessLayerAssemblies, Lifestyle.Scoped);
        }

        // TOOD not okay
        public static IEnumerable<Type> GetCommandTypes()
        {
            return from assembly in _contractAssemblies
                   from type in assembly.GetExportedTypes()
                   where type.Name.EndsWith("Command")
                   select type;
        }

        public static IEnumerable<QueryInfo> GetQueryTypes()
        {
            return from assembly in _contractAssemblies
                   from type in assembly.GetExportedTypes()
                   where QueryInfo.IsQuery(type)
                   select new QueryInfo(type);
        }
    }

    [DebuggerDisplay("{QueryType.Name,nq}")]
    public sealed class QueryInfo
    {
        public QueryInfo(Type queryType)
        {
            QueryType = queryType;
            ResultType = DetermineResultTypes(queryType).Single();
        }

        public Type QueryType { get; }

        public Type ResultType { get; }

        public static bool IsQuery(Type type) => DetermineResultTypes(type).Any();

        private static IEnumerable<Type> DetermineResultTypes(Type type)
        {
            return from interfaceType in type.GetInterfaces()
                   where interfaceType.IsGenericType
                   where interfaceType.GetGenericTypeDefinition() == typeof(IQuery<>)
                   select interfaceType.GetGenericArguments()[0];
        }
    }
}