using Autofac;
using Splat.Autofac;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Utility.Common;
using Utility.WPF;
using Utility.WPF.Controls.Meta;

namespace Utility.WPF.Demo.Trees
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
                Content = new AssemblyViewControl(typeof(ConnectionsUserControl).Assembly)
            }.Show();
        }
    }
}
