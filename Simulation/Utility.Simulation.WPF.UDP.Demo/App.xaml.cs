using DryIoc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Utility.Collections;
using Utility.Nodify.Core;
using Utility.Nodify.Demo;
using Utility.Nodify.Demo.Infrastructure;
using Utility.Nodify.Operations;
using Utility.Nodify.Operations.Infrastructure;

namespace Utility.Simulation.WPF.UDP.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var container = Bootstrapper.Build();

            container.Register<Diagram, Diagram1>();
            container.Register<IOperationsFactory, DynamicOperationsFactory>();

            container.RegisterMany<GraphController>();
            container.RegisterMany<UDPController>();
            container.RegisterMany<ResetController>();

            container.RegisterMany<GraphOperation>();
            container.RegisterMany<UDPOperation>();
            container.RegisterMany<ResetOperation>();
            container.RegisterInstance(SynchronizationContext.Current);

            ThreadSafeObservableCollection<Message>.Context = SynchronizationContext.Current;

            base.OnStartup(e);

            //Window window = new() { Content = new MainViewModel { Player = playerViewModel, Rate = rateViewModel, Progress = progressViewModel } };

            DockWindow dockWindow = new()
            {
                DataContext = container.Resolve<Utility.Nodify.Engine.ViewModels.MainViewModel>()
            };

            //window.Show();
            dockWindow.Show();

            //var graphWindow = new Window { Content = new GraphUserControl(container) };

            //graphWindow.Show();
        }
    }
}
