using Autofac;
using Splat.Autofac;
using System.Windows;
using Utility.Common;
using Utility.WPF;

namespace Utility.WPF.Demo.Adorners
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
               Resolver.Instance.AutoRegister();

            new Window
            {
                WindowState = WindowState.Maximized,
                Content = new UserControlsGrid()
            }.Show();
        }
    }
}
