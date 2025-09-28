using DryIoc;
using Splat;
using System;
using System.Reactive.Concurrency;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric.Dependencies;
using Utility.Models;
using Utility.Nodify.Base.Abstractions;
using Utility.Nodify.Engine;
using Utility.Nodify.Generator.Services;
using Utility.Nodify.Models;
using Utility.Nodify.Models.Infrastructure;
using Utility.ServiceLocation;
using Utility.Services;
using Utility.Simulation;

namespace Utility.Nodify.Demo.Infrastructure
{
    internal class Registration
    {
        public static void registerGlobals(IRegister register)
        {

            register.Register<IScheduler>(DispatcherScheduler.Current);
            register.Register(() => new PlayBackViewModel());
            register.Register(() => new HistoryViewModel());
            register.Register(() => new MasterPlayViewModel());
            register.Register(() => new MainViewModel());
            register.Register<IPlaybackEngine>(() => new PlaybackEngine(Utility.Enums.Playback.Pause));
            register.Register(() => new PlaybackService());
            register.Register<IFactory<INodeViewModel>>(() => new NodeFactory());
            register.Register<IServiceResolver>(() => new ServiceResolver());
            register.Register<IModelResolver>(() => new BasicModelResolver());
            //register.Register<IObservable<IReadOnlyTree>>(() => new TreeResolver());
            register.Register<ExceptionsViewModel>(() => new ExceptionsViewModel());
            //Globals.Register.Register<ITreeRepository>(() => new TreeRepository(sqliteName));
            Globals.Exceptions.Subscribe(a => Globals.Resolver.Resolve<ExceptionsViewModel>().Collection.Add(a));
            register.Register(() => new DiagramViewModel(initialiseContainer()) { Key = "Master", Arrangement = Utility.Enums.Arrangement.UniformRow });
        }

        private static IContainer initialiseContainer()
        {
            var container = new Container(DiConfiguration.SetRules);
            Locator.CurrentMutable.RegisterConstant<IContainer>(container);
            container.Register<IDiagramFactory, DiagramFactory2>();
            _ = Bootstrapper.Build(container);
            //container.Register<IConverter, Converter>();
            return container;
        }


        public static void initialise(IMutableDependencyResolver resolver)
        {
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
