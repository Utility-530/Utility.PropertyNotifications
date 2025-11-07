using System.Windows;
using Autofac;
using Splat.Autofac;

namespace Utility.WPF.Demo.Clocks
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var builder = new ContainerBuilder();
            var d = typeof(Utility.WPF.Demo.Common.ViewModels.Tick);
            //Resolver.Instance.AutoRegister();
            builder.UseAutofacDependencyResolver();
        }
    }
}