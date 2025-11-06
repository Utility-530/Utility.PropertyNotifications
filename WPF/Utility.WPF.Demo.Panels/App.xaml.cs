using System.Windows;
using Autofac;
using Splat.Autofac;
using Utility.WPF.Meta;

namespace Utility.WPF.Demo.Panels
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var builder = new ContainerBuilder();
            //var d = typeof(Utility.WPF.Demo.Common.ViewModels.Tick);
            //Resolver.Instance.AutoRegister();
            builder.UseAutofacDependencyResolver();

            new Window
            {
                WindowState = WindowState.Maximized,
                Content = new UserControlsGrid()
            }.Show();
        }
    }
}