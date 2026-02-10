using System.Reactive.Subjects;
using Microsoft.CodeAnalysis.CSharp;
using Splat;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Interfaces.NonGeneric.Dependencies;
using Utility.Models;
using Utility.Models.Diagrams;
using Utility.Nodes;
using Utility.Nodify.Base.Abstractions;
using Utility.Nodify.Engine;
using Utility.Nodify.Repository;
using Utility.Repos;
using Utility.Roslyn;
using Utility.ServiceLocation;
using Utility.Services;
using Utility.Simulation;
using Utility.WPF.ComboBoxes.Roslyn;

namespace Utility.Nodify.Transitions.Demo.Infrastructure
{
    internal class Registration
    {
        public static async void registerGlobals(IRegister register)
        {  
            const string diagramKey = "Master";
            var diagramGuid = new Guid("CF96DB2C-16D6-4D87-BD49-7E8B1896AF5A");
            register.Register<CSharpCompilation>(() => Compiler.Compile(Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>()));

            register.Register(() => new PlayBackViewModel());
            register.Register(() => new HistoryViewModel());
            register.Register(() => new MasterPlayViewModel());
            register.Register(() => new MainViewModel());
            register.Register<IPlaybackEngine>(() => new PlaybackEngine(Utility.Enums.Playback.Pause));
            register.Register(() => new PlaybackService());
            //register.Register<IFactory<Interfaces.Exs.IViewModelTree>>(() => new NodeFactory());
            register.Register<IServiceResolver>(() => new ServiceResolver());
            register.Register<ExceptionsViewModel>(() => new ExceptionsViewModel());

            //var container = initialiseContainer();
            //register.Register(container);

            Globals.Exceptions.Subscribe(a => Globals.Resolver.Resolve<ExceptionsViewModel>().Collection.Add(a));
            var repo = new DiagramRepository("../../../Data");
            var diagramViewModel = new DiagramViewModel() { Guid = diagramGuid, Name = diagramKey, Arrangement = Utility.Enums.Arrangement.UniformRow };
            register.Register<IDiagramViewModel>(diagramViewModel);
            //container.RegisterInstance<IDiagramViewModel>(diagramViewModel);
            register.Register<IDiagramFactory, DiagramFactory>();
            register.Register<IViewModelFactory, ViewModelFactory>();
            register.Register<IMenuFactory, MenuFactory>();
            register.Register<IEnumerableFactory, NodeFilter>();
            register.Register<IFactory<INodeViewModel>, NodeFactory>();
            register.Register<Subject<Type>>();

            await Globals.Resolver.Resolve<IDiagramFactory>().Build(diagramViewModel);

            repo.Track(diagramViewModel);
            repo.Convert(diagramViewModel);
            //new Tracker().Track(diagramViewModel);
        }

        //public static IContainer initialiseContainer()
        //{
        //    var container = new Container(DiConfiguration.SetRules);
        //    Locator.CurrentMutable.RegisterConstant<IContainer>(container);

    
      
        //    container.Register<IMenuFactory, MenuFactory>();
        //    _ = Utility.Nodify.ViewModels.Infrastructure.Bootstrapper.Build(container);

        //    return container;
        //}
    }
}
