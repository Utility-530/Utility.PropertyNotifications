using Chronic;
using DryIoc;
using Nodify;
using Splat;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utility.Commands;
using Utility.Extensions;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Interfaces.NonGeneric.Dependencies;
using Utility.Models;
using Utility.Models.Diagrams;
using Utility.Nodes;
using Utility.Nodify.Engine.Infrastructure;
using Utility.Nodify.Generator.Services;
using Utility.Nodify.Transitions.Demo.NewFolder;
using Utility.Nodify.ViewModels;
using Utility.ServiceLocation;
using Utility.Simulation;
using IConverter = Utility.Nodify.Engine.Infrastructure.IConverter;
using ServiceResolver = Utility.Services.ServiceResolver;
using MainViewModel = Utility.Nodify.Transitions.Demo.Infrastructure.MainViewModel;
using Utility.Nodify.Transitions.Demo.Infrastructure;

namespace Utility.Nodify.Transitions.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            GlobalRegistration.registerGlobals(Globals.Register);
            initialise(Locator.CurrentMutable);

            FactoryOne.build(Globals.Resolver.Resolve<IModelResolver>());
            FactoryOne.connect(Globals.Resolver.Resolve<IServiceResolver>(), Globals.Resolver.Resolve<IModelResolver>());
            FactoryOne.initialise(Globals.Resolver.Resolve<IModelResolver>());

            var window = new Window()
            {
                Content = Globals.Resolver.Resolve<MainViewModel>()
            };
            window.Show();
            base.OnStartup(e);
        }


        private void initialise(IMutableDependencyResolver resolver)
        {
            resolver.RegisterLazySingleton(() => new CollectionViewService() { Name = nameof(CollectionViewService) });
        }


    }




}
