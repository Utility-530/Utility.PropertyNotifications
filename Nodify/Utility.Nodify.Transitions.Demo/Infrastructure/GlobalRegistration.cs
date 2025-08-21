using DryIoc;
using Splat;
using System.Reactive.Concurrency;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric.Dependencies;
using Utility.Models;
using Utility.Nodify.Engine.Infrastructure;
using Utility.Nodify.ViewModels;
using Utility.ServiceLocation;
using Utility.Services;
using Utility.Simulation;

namespace Utility.Nodify.Transitions.Demo.Infrastructure
{
    internal class GlobalRegistration
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
            register.Register<IFactory<INode>>(() => new NodeFactory());
            register.Register<IServiceResolver>(() => new ServiceResolver());
            register.Register<IModelResolver>(() => new ModelResolver());
            register.Register<ExceptionsViewModel>(() => new ExceptionsViewModel());
            Globals.Exceptions.Subscribe(a => Globals.Resolver.Resolve<ExceptionsViewModel>().Collection.Add(a));
            register.Register(() => new DiagramViewModel(initialiseContainer()) { Key = "Master", Arrangement = Utility.Enums.Arrangement.UniformRow });
        }

        private static IContainer initialiseContainer()
        {
            var container = new Container(DiConfiguration.SetRules);
            Locator.CurrentMutable.RegisterConstant<IContainer>(container);
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
