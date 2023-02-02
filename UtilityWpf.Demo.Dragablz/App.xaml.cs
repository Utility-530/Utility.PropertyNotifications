using Autofac;
using Splat.Autofac;
using System.Windows;
using Utility.Common;

namespace UtilityWpf.Demo.Dragablz
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var builder = new ContainerBuilder();
            //var d = typeof(UtilityWpf.Demo.Common.ViewModels.Tick);
            builder.AutoRegister();
            builder.UseAutofacDependencyResolver();
        }
    }
}