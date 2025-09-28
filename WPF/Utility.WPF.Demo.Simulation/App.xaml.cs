using Chronic;
using CsvHelper;
using Splat;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Utility.Commands;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Models;
using Utility.Nodes;
using Utility.Services;
using Utility.Simulation;

namespace Utility.WPF.Demo.Simulation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Locator.CurrentMutable.RegisterConstant<IScheduler>(DispatcherScheduler.Current);
            Locator.CurrentMutable.RegisterLazySingleton(() => new PlayBackViewModel());
            Locator.CurrentMutable.RegisterLazySingleton(() => new HistoryViewModel());
            Locator.CurrentMutable.RegisterLazySingleton(() => new MasterPlayViewModel());
            Locator.CurrentMutable.RegisterLazySingleton(() => new CommandsViewModel());
            Locator.CurrentMutable.RegisterLazySingleton(() => new ExampleViewModel());
            Locator.CurrentMutable.RegisterLazySingleton<IPlaybackEngine>(() => new PlaybackEngine());
            Locator.CurrentMutable.RegisterLazySingleton<PlaybackService>(() => new PlaybackService());
            Locator.CurrentMutable.RegisterLazySingleton<IFactory<INode>>(() => new NodeFactory());
            //Locator.CurrentMutable.RegisterConstant(new ObervableFactory());
            //Locator.CurrentMutable.RegisterLazySingleton(() => new PlayBackViewModel());

            _ = Utility.Globals.UI;

            var window = new Window()
            {
                Content = Locator.Current.GetService<ExampleViewModel>()
            };
            window.Show();
            base.OnStartup(e);
        }
    }


    public class CommandsViewModel
    {
        public int GridRow => 0;
        public ICommand AddCommand { get; } = new Command(() => Enumerable.Repeat<MethodAction>(new(null, null, null), 5).ForEach(a => Locator.Current.GetService<IPlaybackEngine>().OnNext(a)));
    }

    public class ExampleViewModel
    {


        public object[] Collection => [Locator.Current.GetService<CommandsViewModel>(), Locator.Current.GetService<MasterPlayViewModel>()];
    }

    //public class ObervableFactory
    //{
    //    public ObervableFactory()
    //    {
    //        Observable.Interval(TimeSpan.FromSeconds(0.2)).Subscribe(a =>
    //        {

    //        });
    //    }
    //}

    public class NodeFactory : IFactory<INode>
    {
        public INode Create(object config)
        {
            return new NodeViewModel() { Data = config };
        }
    }

}
