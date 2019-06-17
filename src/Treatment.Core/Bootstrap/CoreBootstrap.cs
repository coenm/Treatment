namespace Treatment.Core.Bootstrap
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using FluentValidation;
    using Helpers.FileSystem;
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
    using Treatment.Helpers.Guards;

    // This class allows registering all types that are defined in the business layer, and are shared across
    // all applications that use this layer (WCF and Web API). For simplicity, this class is placed inside
    // this assembly, but this does couple the business layer assembly to the used container. If this is a
    // concern, create a specific BusinessLayer.Bootstrap project with this class.
    public static class CoreBootstrap
    {
        private static readonly Assembly[] ContractAssemblies = { typeof(IQuery<>).Assembly };
        private static readonly Assembly[] BusinessLayerAssemblies = { Assembly.GetExecutingAssembly() };

        [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod", Justification = "Readability")]
        public static void Bootstrap([NotNull] Container container)
        {
            Guard.NotNull(container, nameof(container));

            if (container.Options.DefaultScopedLifestyle == null)
                container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            RegisterFluentValidationValidators(container);

            container.Register(typeof(ICommandHandler<>), BusinessLayerAssemblies, Lifestyle.Scoped);

            RegisterCommandValidationCommandHandlerDecorators(container);

            // container.RegisterDecorator(typeof(ICommandHandler<>), typeof(AuthorizationCommandHandlerDecorator<>));
            container.Register(typeof(IQueryHandler<,>), BusinessLayerAssemblies, Lifestyle.Scoped);

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
            container.RegisterSingleton<ICommandDispatcher, CommandDispatcher>();
        }

        // TODO: use ICommand interface instead of EndsWith "Command"
        public static IEnumerable<Type> GetCommandTypes()
        {
            return from assembly in ContractAssemblies
                from type in assembly.GetExportedTypes()
                where type.Name.EndsWith("Command")
                select type;
        }

        public static IEnumerable<QueryInfo> GetQueryTypes()
        {
            return from assembly in ContractAssemblies
                from type in assembly.GetExportedTypes()
                where QueryInfo.IsQuery(type)
                select new QueryInfo(type);
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
            container.Register(typeof(IValidator<>), BusinessLayerAssemblies, Lifestyle.Scoped);
        }
    }
}
