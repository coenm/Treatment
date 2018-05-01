namespace Treatment.Console.Bootstrap
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using SimpleInjector;
    using SimpleInjector.Lifestyles;

    using Treatment.Console.Console;
    using Treatment.Console.CrossCuttingConcerns;
    using Treatment.Console.Decorators;
    using Treatment.Contract;
    using Treatment.Contract.Commands;
    using Treatment.Contract.Plugin.FileSearch;
    using Treatment.Core;
    using Treatment.Core.Interfaces;
    using Treatment.Core.Statistics;

    public interface IHoldOnExitOption
    {
        bool HoldOnExit { get; }
    }

    public interface ISearchProviderNameOption
    {
        string SearchProviderName { get; }
    }

    public interface IDryRunOption
    {
        bool IsDryRun { get; }
    }

    public interface IVerboseOption
    {
        VerboseLevel Level { get; }
    }

    public enum VerboseLevel
    {
        High,
        Medium,
        Low,
        Null,
    }

    class DefaultOptions : IVerboseOption, IDryRunOption, IHoldOnExitOption, ISearchProviderNameOption
    {
        public VerboseLevel Level => VerboseLevel.Null;

        public bool IsDryRun => false;

        public bool HoldOnExit => false;

        public string SearchProviderName => string.Empty;
    }

    public class StaticOptions : IVerboseOption, IDryRunOption, IHoldOnExitOption, ISearchProviderNameOption
    {
        public StaticOptions(
            VerboseLevel level,
            bool isDryRun,
            bool holdOnExit,
            string searchProviderName)
        {
            IsDryRun = isDryRun;
            HoldOnExit = holdOnExit;
            SearchProviderName = searchProviderName;
            Level = level;
        }
        public VerboseLevel Level { get; }

        public bool IsDryRun { get; }

        public bool HoldOnExit { get; }

        public string SearchProviderName { get; }
    }


    public class Bootstrapper2
    {
        public Bootstrapper2()
        {
            Container = new Container();
        }

        public Container Container { get; }

        public object GetCommandHandler(Type commandType) =>
            Container.GetInstance(typeof(ICommandHandler<>).MakeGenericType(commandType));

        public object GetQueryHandler(Type queryType) =>
            Container.GetInstance(CreateQueryHandlerType(queryType));

        public TResult ExecuteQuery<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = Container.GetInstance(handlerType);

            return handler.Handle((dynamic)query);
        }

        public IEnumerable<Type> GetCommandTypes() => CoreBootstrap.GetCommandTypes();

        public IEnumerable<Type> GetQueryAndResultTypes()
        {
            var queryTypes = CoreBootstrap.GetQueryTypes().Select(q => q.QueryType);
            var resultTypes = CoreBootstrap.GetQueryTypes().Select(q => q.ResultType).Distinct();
            return queryTypes.Concat(resultTypes);
        }

        public IDisposable StartSession()
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
                                        ctx => Container.GetInstance<IVerboseOption>().Level != VerboseLevel.Null );

            Container.RegisterDecorator(
                                        typeof(IFileSearch),
                                        typeof(VerboseFileSearchDecorator),
                                        Lifestyle.Scoped,
                                        ctx => Container.GetInstance<IVerboseOption>().Level != VerboseLevel.Null);




            //
            // Container.RegisterDecorator(
            //                              typeof(ICommandHandler<>),
            //                              typeof(SetFileSearchSelectorCommandHandlerDecorator<>),
            //                              Lifestyle.Scoped,
            //                              context => typeof(IDirectoryProperty).IsAssignableFrom(context.ServiceType.GetGenericArguments()[0]));
            //


            //tmp
            Container.RegisterSingleton<ISummaryWriter, FakeSummaryWriter>();


            Container.Register<IRootDirSanitizer, RemoveRootDirSanitizer>(Lifestyle.Scoped);


            RegisterPlugins();
            // todo register the rest outside core
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

        private Type CreateQueryHandlerType(Type queryType) =>
            typeof(IQueryHandler<,>).MakeGenericType(queryType, new QueryInfo(queryType).ResultType);



        private void RegisterPlugins()
        {
            var pluginDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);

            var pluginAssemblies = new DirectoryInfo(pluginDirectory)
                                   .GetFiles()
                                   .Where(file =>
                                              file.Name.StartsWith("Treatment.Plugin.")
                                              &&
                                              file.Extension.ToLower() == ".dll")
                                   .Select(file => Assembly.Load(AssemblyName.GetAssemblyName(file.FullName)));

            Container.RegisterPackages(pluginAssemblies);
        }
    }
}