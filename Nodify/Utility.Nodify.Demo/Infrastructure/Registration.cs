using System;
using System.Reactive.Concurrency;
using DryIoc;
using Splat;
using TreeCollections;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric.Dependencies;
using Utility.Models;
using Utility.Models.Diagrams;
using Utility.Nodes;
using Utility.Nodify.Base.Abstractions;
using Utility.Nodify.Engine;
using Utility.Nodify.Generator.Services;
using Utility.Nodify.ViewModels;
using Utility.Nodify.ViewModels.Infrastructure;
using Utility.Repos;
using Utility.ServiceLocation;
using Utility.Services;
using Utility.Services.Meta;
using Utility.Simulation;

namespace Utility.Nodify.Demo.Infrastructure
{
    internal class Registration
    {
        public static async void registerGlobals(IRegister register)
        {

            register.Register<IScheduler>(() => new SynchronizationContextScheduler(Globals.UI));
            register.Register(() => new PlayBackViewModel());
            register.Register(() => new HistoryViewModel());
            register.Register(() => new MasterPlayViewModel());
            register.Register(() => new MainViewModel());
            register.Register<IPlaybackEngine>(() => new PlaybackEngine(Utility.Enums.Playback.Pause));
            register.Register(() => new PlaybackService());
            register.Register<IDataActivator>(() => new Utility.Nodes.Meta.DataActivator());
            register.Register<IFactory<IViewModelTree>>(() => new NodeFactory());
            register.Register<IServiceResolver>(() => new ServiceResolver());
            register.Register<Utility.Nodes.Meta.NodeInterface>();
            register.Register<INodeSource>(() => new Utility.Nodes.Meta.NodesStore());
            register.Register<ExceptionsViewModel>(() => new ExceptionsViewModel());
            //Globals.Register.Register<ITreeRepository>(() => new TreeRepository(sqliteName));
            Globals.Exceptions.Subscribe(a => Globals.Resolver.Resolve<ExceptionsViewModel>().Collection.Add(a));
            register.Register(() => new DiagramViewModel(initialiseContainer()) { Key = "Master", Arrangement = Utility.Enums.Arrangement.UniformRow });
            register.Register<INodeRoot>(() => new Utility.Nodes.Meta.NodeEngine(new TreeRepository("../../../Data"), new ValueRepository("../../../Data")));

            await new DiagramFactory().Build(Globals.Resolver.Resolve<DiagramViewModel>());
            //new Tracker().Track(Globals.Resolver.Resolve<DiagramViewModel>());
        }

        private static IContainer initialiseContainer()
        {
            var container = new Container(DiConfiguration.SetRules);
            Locator.CurrentMutable.RegisterConstant<IContainer>(container);
            //container.Register<IDiagramFactory, DiagramFactory>();
            container.Register<IMenuFactory, MenuFactory>();
            _ = Bootstrapper.Build(container);
            //container.Register<IConverter, Converter>();
            return container;
        }


        public static void initialise(IMutableDependencyResolver resolver)
        {
            resolver.RegisterLazySingleton<IEnumerableFactory<Method>>(() => new FactoryOne());
            resolver.RegisterLazySingleton(() => new CollectionViewService() { Name = nameof(CollectionViewService) });
            //resolver.RegisterLazySingleton<Utility.Nodify.Operations.Infrastructure.INodeSource>(() => new NodeSource() { });
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
