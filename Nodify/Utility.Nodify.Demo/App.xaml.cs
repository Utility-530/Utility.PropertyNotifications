using DryIoc;
using Splat;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using Utility.Interfaces.Exs;
using Utility.Models;
using Utility.Nodify.Core;
using Utility.Nodify.Engine.Infrastructure;
using Utility.Nodify.ViewModels;
using Utility.Nodify.Generator.Services;
using Utility.Repos;
using IConverter = Utility.Nodify.Engine.Infrastructure.IConverter;
using INodeSource = Utility.Nodify.Operations.Infrastructure.INodeSource;
using ServiceResolver = Utility.Services.ServiceResolver;
using Utility.Nodify.Demo.Infrastructure;
using Utility.Extensions;
using Utility.ServiceLocation;

namespace Utility.Nodify.Demo
{

    public partial class App : Application
    {
        IContainer container;
        //IDescriptor rootDescriptor;

        Guid guid = Guid.Parse("25ee5731-11cf-4fc1-a925-50272fb99bba");

        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();
            initialise();

            var diagram = new Diagram();
            base.OnStartup(e);

            container = new Container(DiConfiguration.SetRules);
            container.Register<INodeSource, NodeSource>();

            _ = Bootstrapper.Build(container);

            container.RegisterInstance<Diagram>(diagram);
            container.Register<IConverter, Converter>();
            DockWindow dockWindow = new()
            {
                DataContext = container.Resolve<MainViewModel>()
            };

            dockWindow.Show();
        }

        private void initialise()
        {
            Utility.Globals.Register.Register< IServiceResolver>(() => new ServiceResolver());
            Locator.CurrentMutable.RegisterLazySingleton(() => new CollectionViewService());

            Utility.Globals.Register.Register<IPlaybackEngine>(new PlaybackEngine());

            var serviceResolver = Utility.Globals.Resolver.Resolve<IServiceResolver>();
            serviceResolver.Connect<PredicateReturnParam, PredicateParam>();
            serviceResolver.Connect<ListInstanceReturnParam, ListInParam>();
            serviceResolver.Connect<ListInstanceReturnParam, ListParam>();
            serviceResolver.Observe<InstanceTypeParam>(new ValueModel<Type>() { Name = "react_to_4", Value = typeof(List<object>) });
            serviceResolver.Observe<FilterParam>(new ValueModel<string>() { Name = "react_to_3", Value = "something" });

        }
        public static class DiConfiguration
        {
            public static Rules SetRules(Rules rules)
            {
                rules = rules
                    .WithDefaultReuse(Reuse.Singleton)
                    .With(FactoryMethod.ConstructorWithResolvableArguments);
                return rules;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //var diagramViewModel = container.Resolve<IDiagramViewModel>();
            //var diagram = container.Resolve<IConverter>().ConvertBack(diagramViewModel);
            //rootDescriptor.Set(diagram);
            //rootDescriptor.Finalise();
            base.OnExit(e);
        }
    }
}
