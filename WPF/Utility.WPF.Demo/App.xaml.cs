using System.Windows;
using Utility.WPF.Controls.Meta;

namespace Utility.WPF.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //Resolver.Instance.AutoRegister();

            new Window
            {
                WindowState = WindowState.Maximized,
                Content = new AssemblyViewsControl()
            }.Show();
        }
    }
}