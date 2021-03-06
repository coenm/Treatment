﻿namespace Treatment.Console.Bootstrap
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using Treatment.Console.Console;
    using Treatment.Console.CrossCuttingConcerns;
    using Treatment.Console.Decorators;
    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Core.Bootstrap;
    using Treatment.Core.Bootstrap.Plugin;
    using Treatment.Core.DefaultPluginImplementation.FileSearch;
    using Treatment.Helpers.FileSystem;

    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class Bootstrapper
    {
        public Bootstrapper()
        {
            Container = new Container();
        }

        public Container Container { get; }

        public object GetCommandHandler(Type commandType) => Container.GetInstance(typeof(ICommandHandler<>).MakeGenericType(commandType));

        public object GetQueryHandler(Type queryType) => Container.GetInstance(CreateQueryHandlerType(queryType));

        public Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default)
        {
            return Container.GetInstance<IQueryProcessor>().ExecuteQueryAsync(query, ct);
        }

        public IEnumerable<Type> GetCommandTypes() => CoreBootstrap.GetCommandTypes();

        public IEnumerable<Type> GetQueryAndResultTypes()
        {
            var queryTypes = CoreBootstrap.GetQueryTypes().Select(q => q.QueryType);
            var resultTypes = CoreBootstrap.GetQueryTypes().Select(q => q.ResultType).Distinct();
            return queryTypes.Concat(resultTypes);
        }

        public virtual IDisposable StartSession()
        {
            return AsyncScopedLifestyle.BeginScope(Container);
        }

        public void Init()
        {
            Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            Container.Options.AllowOverridingRegistrations = true;

            CoreBootstrap.Bootstrap(Container);

            Container.RegisterInstance<IConsole>(ConsoleAdapter.Instance);

            RegisterDefaultOptions();

            Container.RegisterDecorator(
                                        typeof(ICommandHandler<>),
                                        typeof(SetRootDirectoryCommandHandlerDecorator<>),
                                        Lifestyle.Scoped,
                                        ctx => typeof(IDirectoryProperty).IsAssignableFrom(ctx.ServiceType.GetGenericArguments()[0]));

            Container.RegisterDecorator(
                                        typeof(ICommandHandler<>),
                                        typeof(HoldConsoleCommandHandlerDecorator<>),
                                        Lifestyle.Scoped,
                                        ctx => Container.GetInstance<IHoldOnExitOption>().HoldOnExit);

            Container.RegisterDecorator(
                                        typeof(ICommandHandler<>),
                                        typeof(WriteExceptionToConsoleCommandHandlerDecorator<>),
                                        Lifestyle.Scoped);

            Container.RegisterDecorator(
                                        typeof(IFileSystem),
                                        typeof(DryRunFileSystemDecorator),
                                        Lifestyle.Scoped,
                                        ctx => Container.GetInstance<IDryRunOption>().IsDryRun);

            Container.RegisterDecorator(
                                        typeof(IFileSystem),
                                        typeof(VerboseFileSystemDecorator),
                                        Lifestyle.Scoped,
                                        ctx => Container.GetInstance<IVerboseOption>().Level != VerboseLevel.Disabled);

            Container.RegisterDecorator(
                                        typeof(IFileSearch),
                                        typeof(VerboseFileSearchDecorator),
                                        Lifestyle.Scoped,
                                        ctx => Container.GetInstance<IVerboseOption>().Level != VerboseLevel.Disabled);

            Container.Register<IRootDirSanitizer, RemoveRootDirSanitizer>(Lifestyle.Scoped);
            Container.Register<IHoldConsole, HoldConsole>(Lifestyle.Singleton);

            RegisterPlugins();
        }

        public void RegisterDefaultOptions()
        {
            var options = new DefaultOptions();
            Container.Register<IVerboseOption>(() => options, Lifestyle.Scoped);
            Container.Register<IDryRunOption>(() => options, Lifestyle.Scoped);
            Container.Register<IHoldOnExitOption>(() => options, Lifestyle.Scoped);
            Container.Register<ISearchProviderNameOption>(() => options, Lifestyle.Scoped);
        }

        public virtual void VerifyContainer()
        {
            Container.Verify();
        }

        private static Type CreateQueryHandlerType(Type queryType) =>
            typeof(IQueryHandler<,>).MakeGenericType(queryType, new QueryInfo(queryType).ResultType);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RegisterPlugins()
        {
            var pluginAssemblies = PluginFinder.FindPluginAssemblies(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
            Container.RegisterPackages(pluginAssemblies);
        }
    }
}
