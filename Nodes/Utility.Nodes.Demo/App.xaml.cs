using System.Threading;
using System.Windows;
using Utility.Collections;

namespace Utility.Nodes.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        { 
            base.OnStartup(e);
            Collection.Context = SynchronizationContext.Current;
            var window = new Window { Content = new DemoRootNode() };
            window.Show();
        }
    }
}
