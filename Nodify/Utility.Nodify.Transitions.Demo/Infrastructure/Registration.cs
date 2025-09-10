using DryIoc;
using Splat;
using System.Reactive.Concurrency;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric.Dependencies;
using Utility.Models;
using Utility.Nodify.Base.Abstractions;
using Utility.Nodify.Engine;
using Utility.Nodify.Engine.Infrastructure;
using Utility.Nodify.Generator.Services;
using Utility.Nodify.Models;
using Utility.Nodify.Repository;
using Utility.Repos;
using Utility.ServiceLocation;
using Utility.Services;
using Utility.Simulation;
using Utility.Trees.Abstractions;

namespace Utility.Nodify.Transitions.Demo.Infrastructure
{
    internal class Registration
    {
        public static async void registerGlobals(IRegister register)
        {
            const string sqliteName = "O:\\Users\\rytal\\source\\repos\\Utility\\Nodes\\Utility.Nodes.Demo.Editor\\Data\\first_7.sqlite";
            const string diagramKey = "Master";

            register.Register<IScheduler>(DispatcherScheduler.Current);
            register.Register(() => new PlayBackViewModel());
            register.Register(() => new HistoryViewModel());
            register.Register(() => new MasterPlayViewModel());
            register.Register(() => new MainViewModel());
            register.Register<IPlaybackEngine>(() => new PlaybackEngine(Utility.Enums.Playback.Pause));
            register.Register(() => new PlaybackService());
            register.Register<IFactory<INode>>(() => new NodeFactory());
            register.Register<IServiceResolver>(() => new ServiceResolver());
            register.Register<IModelResolver>(() => new BasicModelResolver());
            //register.Register<IObservable<IReadOnlyTree>>(() => new TreeResolver());
            register.Register<ExceptionsViewModel>(() => new ExceptionsViewModel());
            register.Register<ITreeRepository>(() => new TreeRepository(sqliteName));
            //register.Register(() => new DiagramRepository(container, "../../../Data"));
            register.Register(initialiseContainer());

            Globals.Exceptions.Subscribe(a => Globals.Resolver.Resolve<ExceptionsViewModel>().Collection.Add(a));
            var repo = new DiagramRepository(Globals.Resolver.Resolve<DryIoc.IContainer>(), "../../../Data");
            //var diagramViewModel = Globals.Resolver.Resolve<DiagramRepository>().Convert(diagramKey);
            var diagramViewModel = new DiagramViewModel(Globals.Resolver.Resolve<IContainer>()) { Key = diagramKey, Arrangement = Utility.Enums.Arrangement.UniformRow };
            register.Register<DiagramViewModel>(diagramViewModel);
            var d = Globals.Resolver.Resolve<DiagramViewModel>();
            await Globals.Resolver.Resolve<DryIoc.IContainer>().Resolve<IDiagramFactory>().Build(diagramViewModel);

            register.Register(async () =>
            {
                return diagramViewModel;
            });
         

            repo.Track(d);
            repo.Convert(d);
        }

        //public static void initialise(IMutableDependencyResolver resolver)
        //{
        //    //resolver.RegisterLazySingleton(() => new CollectionViewService() { Name = nameof(CollectionViewService) });
        //    //resolver.RegisterLazySingleton<Utility.Nodify.Operations.Infrastructure.INodeSource>(() => new NodeSource() { });
        //}

        public static IContainer initialiseContainer()
        {
            var container = new Container(DiConfiguration.SetRules);
            Locator.CurrentMutable.RegisterConstant<IContainer>(container);
            container.Register<IDiagramFactory, DiagramFactory>();
            container.Register<IConnectionFactory, ConnectionFactory>();
            container.Register<IConnectorFactory, ConnectorFactory>();
            container.Register<IViewModelFactory, ViewModelFactory>();
            container.Register<Utility.Nodify.Operations.Infrastructure.INodeSource, NodeSource>();
            container.Register<IMenuFactory, MenuFactory>();
            container.Register<CollectionViewService>();

            _ = Utility.Nodify.Models.Infrastructure.Bootstrapper.Build(container);
            _ = Utility.Nodify.ViewModels.Infrastructure.Bootstrapper.Build(container);
                       
            //container.Register<IConverter, Converter>();
            return container;
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
    }
}
