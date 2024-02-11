//using Autofac;
using DryIoc;
using Utility.Nodify.Core;
using Utility.Nodify.Demo.Infrastructure;
using Utility.Nodify.Operations;
using System;
using System.Windows;
using Utility.Models;
using Utility.Nodify.Operations.Infrastructure;
using Utility.Nodify.Engine.ViewModels;
using Utility.Infrastructure;

namespace Utility.Nodify.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var container = Bootstrapper.Build();
            container.Register<Diagram, Diagram1>();
            container.Register<BaseViewModel, BooleanViewModel>();
            container.Register<BaseViewModel, ViewModel>();
            container.Register<OperationInterfaceNodeViewModel>();
            container.Register<InterfaceViewModel>();

            container.Register<IOperationsFactory, CustomOperationsFactory>();
            container.Register<IOperationsFactory, InterfaceOperationsFactory>();


            DockWindow dockWindow = new()
            {
                DataContext = container.Resolve<MainViewModel>()
            };

            Window window = new()
            {
                Content = container.Resolve<InterfaceViewModel>()
            };


            dockWindow.Show();
            window.Show();
        }

    }


}
