using DryIoc;
using Utility.Nodify.Core;
using Utility.Nodify.Demo.Infrastructure;
using System;
using System.Windows;
using Utility.Nodify.Operations.Infrastructure;
using System.Threading;
using Utility.Simulation.Infrastructure;
using Utility.Simulation.Service;
using Utility.Simulation.ViewModel;
using Message = Utility.Nodify.Operations.Message;
using Utility.Collections;

namespace Utility.Nodify.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var container = Bootstrapper.Build();


            var dateTimeRange = new TimeTrack(DateTime.UnixEpoch, DateTime.Now);
            var engine = new Utility.Simulation.Service.Engine(dateTimeRange);

            var playerViewModel = new PlayerViewModel(SynchronizationContext.Current);
            var playerengineService = new PlayerEngineService();
            var rateViewModel = new RateViewModel(0.1, 8, 5);
            var progressViewModel = new ProgressViewModel(SynchronizationContext.Current, dateTimeRange.Start, dateTimeRange.End);

            //container.RegisterInstance(dateTimeRange);

            container.RegisterInstance<object>(engine);
            container.RegisterInstance<object>(playerViewModel);
            container.RegisterInstance<object>(playerengineService);
            container.RegisterInstance<object>(rateViewModel);
            container.RegisterInstance<object>(progressViewModel);


            container.Register<Diagram, Diagram1>();
            container.Register<IOperationsFactory, DynamicOperationsFactory>();

            ThreadSafeObservableCollection<Message>.Context = SynchronizationContext.Current;

  
            base.OnStartup(e);

            Window window = new (){ Content = new MainViewModel { Player = playerViewModel, Rate = rateViewModel, Progress = progressViewModel } };

            DockWindow dockWindow = new()
            {
                DataContext = container.Resolve<Utility.Nodify.Engine.ViewModels.MainViewModel>()
            };

            window.Show();
            dockWindow.Show();
        }
    }



}