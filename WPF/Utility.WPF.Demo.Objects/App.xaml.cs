using Autofac;
using Splat.Autofac;
using System.Windows;
using Utility.Common;
using Utility.WPF.Controls.Meta;
using Utility.WPF.Meta;
using Utility.WPF.Templates;

namespace Utility.WPF.Demo.Objects
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

            new Window
            {
                WindowState = WindowState.Maximized,
                Content = new AssemblyViewControl(typeof(ObjectUserControl).Assembly)
                //Content = new ResourceDictionariesGrid()
            }.Show();
        }
    }
}