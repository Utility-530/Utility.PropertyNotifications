using Autofac;
using Splat.Autofac;
using System.Windows;
using Utility.Common;

namespace Utility.WPF.Demo.Hybrid
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
        }
    }
}