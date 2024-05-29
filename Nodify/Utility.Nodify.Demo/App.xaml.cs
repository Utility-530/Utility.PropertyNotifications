using DryIoc;
using Utility.Nodify.Core;
using System;
using System.Windows;
using Utility.Nodify.Operations.Infrastructure;
using Utility.Nodify.Engine.ViewModels;
using Utility.Descriptors.Repositorys;
using Splat;
using Utility.Nodify.Engine.Infrastructure;
using IConverter = Utility.Nodify.Engine.Infrastructure.IConverter;
using Utility.Descriptors;
using Utility.Interfaces;
using Utility.Extensions;

namespace Utility.Nodify.Demo
{

    public partial class App : Application
    {
        IContainer container;
        IValueDescriptor rootDescriptor;

        Guid guid = Guid.Parse("25ee5731-11cf-4fc1-a925-50272fb99bba");

        protected override async void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();

            Locator.CurrentMutable.RegisterConstant<ITreeRepository>(TreeRepository.Instance);


            var rootDescriptor = await DescriptorFactory.CreateRoot(typeof(Diagram), guid, name: "diagram_test");
            var diagram = rootDescriptor.Get<Diagram>();
            rootDescriptor.Initialise();
            base.OnStartup(e);

            container = new Container(DiConfiguration.SetRules);
            container.Register<INodeSource, NodeSource>();

            _ = Bootstrapper.Build(container);

            container.RegisterInstance<Diagram>(diagram);
            container.Register<IConverter, Converter>();

            container.Resolve<Utility.Nodify.Operations.Resolver>();

            DockWindow dockWindow = new()
            {
                DataContext = container.Resolve<MainViewModel>()
            };

            dockWindow.Show();
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
            var diagramViewModel = container.Resolve<IDiagramViewModel>();
            var diagram = container.Resolve<IConverter>().ConvertBack(diagramViewModel);
            rootDescriptor.Set(diagram);
            rootDescriptor.Finalise();
            base.OnExit(e);
        }
    }
}
