using Chronic;
using DryIoc;
using Splat;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Utility.Commands;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Models;
using Utility.Nodes;
using Utility.Nodify.Engine.Infrastructure;
using Utility.Nodify.Generator.Services;
using Utility.Nodify.ViewModels;
using Utility.ServiceLocation;
using Utility.Simulation;
using IConverter = Utility.Nodify.Engine.Infrastructure.IConverter;
using ServiceResolver = Utility.Services.ServiceResolver;
using Utility.Extensions;
using Utility.Interfaces.NonGeneric.Dependencies;
using Utility.Models.Diagrams;


namespace Utility.Nodify.Transitions.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            registerGlobals(Globals.Register);
            initialise(Locator.CurrentMutable);
            connect(Globals.Resolver.Resolve<IServiceResolver>());

            var window = new Window()
            {
                Content = Globals.Resolver.Resolve<MainViewModel>()
            };
            window.Show();
            base.OnStartup(e);
        }

        private static void registerGlobals(IRegister register)
        {
            register.Register<IScheduler>(DispatcherScheduler.Current);
            register.Register(() => new PlayBackViewModel());
            register.Register(() => new HistoryViewModel());
            register.Register(() => new MasterPlayViewModel());
            register.Register(() => new MainViewModel());
            register.Register<IPlaybackEngine>(() => new PlaybackEngine(Utility.Enums.Playback.Pause));
            register.Register<Services.PlaybackService>(() => new Services.PlaybackService());
            register.Register<IFactory<INode>>(() => new NodeFactory());
            register.Register<IServiceResolver>(() => new ServiceResolver());
            register.Register(() => new DiagramViewModel(initialiseContainer()));
        }

        private static void connect(IServiceResolver serviceResolver)
        {
            serviceResolver.Connect<PredicateReturnParam, PredicateParam>();
            serviceResolver.Connect<ListInstanceReturnParam, ListInParam>();
            serviceResolver.Connect<ListInstanceReturnParam, ListParam>();
            serviceResolver.Observe<InstanceTypeParam>(new ValueModel<Type>() { Name = "react_to_4", Value = typeof(List<object>) });
            serviceResolver.Observe<FilterParam>(new ValueModel<string>() { Name = "react_to_3", Value = "something" });
        }

        private void initialise(IMutableDependencyResolver resolver)
        {
            resolver.RegisterLazySingleton(() => new CollectionViewService());
        }

        private static IContainer initialiseContainer()
        {
            var container = new DryIoc.Container(DiConfiguration.SetRules);
            //container.Register<INodeSource, NodeSource>();

            _ = Bootstrapper.Build(container);

            //container.RegisterInstance<Diagram>(diagram);
            container.Register<IConverter, Converter>();

            return container;
        }
    }

    class DiConfiguration
    {
        public static Rules SetRules(Rules rules)
        {
            rules = rules
                .WithDefaultReuse(Reuse.Singleton)
                .With(FactoryMethod.ConstructorWithResolvableArguments);
            return rules;
        }
    }

    public class CommandsViewModel
    {
        public int GridRow => 0;
        public ICommand AddCommand { get; } = new Command(() => Enumerable.Repeat<MethodAction>(new(null, null, null), 5).ForEach(a => Locator.Current.GetService<IPlaybackEngine>().OnNext(a)));
    }

    public class MainViewModel
    {
        object[] collection = [Globals.Resolver.Resolve<DiagramViewModel>(), Globals.Resolver.Resolve<MasterPlayViewModel>()];

        public object[] Collection => collection;
    }

    public class NodeFactory : IFactory<INode>
    {
        public INode Create(object config)
        {
            return new Node() { Data = config };
        }
    }
}
