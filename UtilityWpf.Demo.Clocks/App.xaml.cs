using Autofac;
using Splat.Autofac;
using System.Windows;
using Utility.Common;

namespace UtilityWpf.Demo.Clocks
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
            Resolver.Instance.AutoRegister();
            builder.UseAutofacDependencyResolver();
        }
    }
}