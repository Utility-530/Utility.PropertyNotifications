using DryIoc;
using Splat;
using System.Reactive.Concurrency;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric.Dependencies;
using Utility.Models;
using Utility.Nodify.Engine;
using Utility.Nodify.Engine.Infrastructure;
using Utility.Nodify.Generator.Services;
using Utility.Nodify.Repository;
using Utility.Nodify.ViewModels;
using Utility.Repos;
using Utility.ServiceLocation;
using Utility.Services;
using Utility.Simulation;
using Utility.Trees.Abstractions;

namespace Utility.Nodify.Transitions.Demo.Infrastructure
{
    internal class Registration
    {
        public static void registerGlobals(IRegister register)
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
            register.Register<IObservable<IReadOnlyTree>>(() => new TreeResolver());
            register.Register<ExceptionsViewModel>(() => new ExceptionsViewModel());
            register.Register<ITreeRepository>(() => new TreeRepository(sqliteName));
            register.Register(() => new DiagramRepository("../../../Data"));
            register.Register(initialiseContainer());

            Globals.Exceptions.Subscribe(a => Globals.Resolver.Resolve<ExceptionsViewModel>().Collection.Add(a));
            register.Register(() =>
            {
                var diagramViewModel = Globals.Resolver.Resolve<DiagramRepository>().Convert(diagramKey);
                diagramViewModel ??= new DiagramViewModel(Globals.Resolver.Resolve<IContainer>()) { Key = diagramKey, Arrangement = Utility.Enums.Arrangement.UniformRow };
                return diagramViewModel;
            });
            var repo = new DiagramRepository("../../../Data");
            var d = Globals.Resolver.Resolve<DiagramViewModel>();
            repo.Track(d);
        }

        public static void initialise(IMutableDependencyResolver resolver)
        {
            resolver.RegisterLazySingleton(() => new CollectionViewService() { Name = nameof(CollectionViewService) });
            resolver.RegisterLazySingleton<Utility.Nodify.Operations.Infrastructure.INodeSource>(() => new NodeSource() { });
        }

        public static IContainer initialiseContainer()
        {
            var container = new Container(DiConfiguration.SetRules);
            Locator.CurrentMutable.RegisterConstant<IContainer>(container);
            container.Register<IDiagramFactory, DiagramFactory>();

            _ = Bootstrapper.Build(container);
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
