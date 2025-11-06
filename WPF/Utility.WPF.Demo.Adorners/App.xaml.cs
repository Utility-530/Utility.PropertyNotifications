using System.Windows;
using Utility.WPF.Meta;

namespace Utility.WPF.Demo.Adorners
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
                Content = new UserControlsGrid()
            }.Show();
        }
    }
}
