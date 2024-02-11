using Autofac;
using Splat.Autofac;
using System.Windows;
using Utility.Common;
using Utility.WPF.Controls.Meta;

namespace Utility.WPF.Demo.Controls
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var builder = new ContainerBuilder();
            Resolver.Instance.AutoRegister();
            builder.UseAutofacDependencyResolver();

            new Window
            {
                WindowState = WindowState.Maximized,
                Content = new AssemblyViewControl(typeof(App).Assembly)
            }.Show();
        }
    }
}