using System.Windows;
using Utility.WPF.Controls.Meta;

namespace Utility.WPF.Demo.Lists
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var d = typeof(Utility.WPF.Demo.Common.ViewModels.Tick);
            //Resolver.Instance.AutoRegister();
            //builder.UseAutofacDependencyResolver();
            SQLitePCL.Batteries.Init();
            new Window
            {
                WindowState = WindowState.Maximized,
                Content = new AssemblyViewControl(typeof(App).Assembly)
            }.Show();
        }
    }
}