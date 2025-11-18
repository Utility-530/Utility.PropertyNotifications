using System;
using System.Windows;
using System.Windows.Navigation;
using Utility.WPF.Controls.Meta;
using Utility.WPF.Reactives;

namespace Utility.WPF.Demo.Trees
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var window = new Window();
 
            window.Content = new object();
            new Window
            {
                WindowState = WindowState.Maximized,
                Content = new AssemblyViewControl(typeof(ConnectionsUserControl).Assembly)
            }.Show();
        }

        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            _ = SynchronizationContextScheduler.Instance;
            base.OnLoadCompleted(e);
        }
    }
}