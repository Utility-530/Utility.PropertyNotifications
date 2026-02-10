using System;
using System.Reactive.Concurrency;
using Microsoft.CodeAnalysis.CSharp;
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
using Utility.Nodify.Demo.Services;
using Utility.Nodify.Engine;
using Utility.Repos;
using Utility.Roslyn;
using Utility.ServiceLocation;
using Utility.Services;
using Utility.Services.Meta;
using Utility.Simulation;

namespace Utility.Nodify.Demo.Infrastructure
{
    internal class Registration
    {
        static readonly Guid diagramGuid = new Guid("014A058C-90B1-46D4-BD4F-DCE5B44858AE");
        public static async void registerGlobals(IRegister register)
        {
            register.Register<CSharpCompilation>(() => Compiler.Compile(Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>()));

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
            register.Register<IViewModelFactory, ViewModelFactory>();
            //Globals.Register.Register<ITreeRepository>(() => new TreeRepository(sqliteName));
            Globals.Exceptions.Subscribe(a => Globals.Resolver.Resolve<ExceptionsViewModel>().Collection.Add(a));
            register.Register<IDiagramViewModel>(() => new DiagramViewModel() { Guid = diagramGuid, Name = "Master", Arrangement = Utility.Enums.Arrangement.UniformRow });
            register.Register<INodeRoot>(() => new Utility.Nodes.Meta.NodeEngine(new TreeRepository("../../../Data"), new ValueRepository("../../../Data")));
            register.Register<IMenuFactory, MenuFactory>();
            await new DiagramFactory().Build(Globals.Resolver.Resolve<IDiagramViewModel>());
            //new Tracker().Track(Globals.Resolver.Resolve<DiagramViewModel>());
        }


        public static void initialise(IMutableDependencyResolver resolver)
        {
            resolver.RegisterLazySingleton<IEnumerableFactory<Method>>(() => new FactoryOne());
            resolver.RegisterLazySingleton(() => new CollectionViewService() { Name = nameof(CollectionViewService) });
            //resolver.RegisterLazySingleton<Utility.Nodify.Operations.Infrastructure.INodeSource>(() => new NodeSource() { });
        }
    }
}
