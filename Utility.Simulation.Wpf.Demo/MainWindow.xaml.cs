using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Windows;
using Utility.Simulation.Infrastructure;
using Utility.Simulation.Service;
using Utility.Simulation.ViewModel;

namespace Utility.Simulation.Wpf.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var dateTimeRange = new TimeTrack(DateTime.UnixEpoch, DateTime.Now);
            var engine = new Engine(dateTimeRange);
   
            var playerViewModel = new PlayerViewModel(SynchronizationContext.Current);
            var playerengineService = new PlayerEngineService();
            var rateViewModel = new RateViewModel(0.1, 8, 5);
            var progressViewModel = new ProgressViewModel(SynchronizationContext.Current, dateTimeRange.Start, dateTimeRange.End);

            playerViewModel.Subscribe(playerengineService);
            rateViewModel.Subscribe(playerengineService);
            playerengineService.Subscribe(engine);
            engine.Subscribe(progressViewModel);
            engine.Subscribe(playerViewModel);

            RateContentControl.Content = rateViewModel;
            PlayerContentControl.Content = playerViewModel;
            ProgressContentControl.Content = progressViewModel;
        }
    }
}
