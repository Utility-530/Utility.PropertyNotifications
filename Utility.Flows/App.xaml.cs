using CoffeeFlow.WPF.Infrastructure;
using System.Threading;
using System.Windows;
using Utility.Collections;

namespace CoffeeFlow.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
       
            Collection.Context = SynchronizationContext.Current;
            Globals.Instance.Dispatcher = App.Current.Dispatcher;

            Globals.Instance.Main = new ViewModel.MainViewModel();
            Globals.Instance.Network = new ViewModel.NetworkViewModel();
      

            base.OnStartup(e);
            var window = new MainWindow();
            Globals.Instance.MainWindow = window;
            window.Show();
        }
    }
}
