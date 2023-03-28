using Autofac;
using Splat.Autofac;
using System.Windows;
using Utility.Common;
using UtilityWpf;

namespace Utility.WPF.Demo.Lists
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
 
            var d = typeof(UtilityWpf.Demo.Common.ViewModels.Tick);
            Resolver.Instance.AutoRegister();
            //builder.UseAutofacDependencyResolver();

            new Window
            {
                WindowState = WindowState.Maximized,
                Content = new UserControlsGrid()
            }.Show();
        }
    }
}