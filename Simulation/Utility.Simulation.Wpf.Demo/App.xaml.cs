using DryIoc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Utility.Simulation.Infrastructure;
using Utility.Simulation.Service;
using Utility.Simulation.ViewModel;

namespace Utility.Simulation.Wpf.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            var container = new Container();


            var dateTimeRange = new TimeTrack(DateTime.UnixEpoch, DateTime.Now);
            var engine = new Engine(dateTimeRange);

            var playerViewModel = new PlayerViewModel(SynchronizationContext.Current);
            var playerengineService = new PlayerEngineService();
            var rateViewModel = new RateViewModel(0.1, 8, 5);
            var progressViewModel = new ProgressViewModel(SynchronizationContext.Current, dateTimeRange.Start, dateTimeRange.End);

            //container.RegisterInstance(dateTimeRange);
            //container.RegisterInstance(engine);
            //container.RegisterInstance(playerViewModel);
            //container.RegisterInstance(playerengineService);
            //container.RegisterInstance(rateViewModel);
            //container.RegisterInstance(progressViewModel);

            //container.Register<Connection<PlayerViewModel, PlayerEngineService>>();
            //container.Register<Connection<RateViewModel, PlayerEngineService>>();
            //container.Register<Connection<PlayerEngineService, Engine>>();
            //container.Register<Connection<Engine, ProgressViewModel>>();
            //container.Register<Connection<Engine, PlayerViewModel>>();



            playerViewModel.Subscribe(playerengineService);
            rateViewModel.Subscribe(playerengineService);
            playerengineService.Subscribe(engine);
            engine.Subscribe(progressViewModel);
            engine.Subscribe(playerViewModel);



            base.OnStartup(e);

            var window = new Window { Content = new MainViewModel { Player = playerViewModel, Rate = rateViewModel, Progress = progressViewModel } };
            window.Show();
        }
    }

    //public class Connection<TInput, TOutput>
    //{

    //}

    public class MainViewModel
    {
        public PlayerViewModel Player { get; set; }
        public RateViewModel Rate { get; set; }
        public ProgressViewModel Progress { get; set; }
    }
}
