using System.Windows;
using Utility.WPF.Controls.Meta;

namespace Utility.WPF.Demo.SplitButtons
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            new Window
            {
                WindowState = WindowState.Maximized,
                Content = new AssemblyViewControl(typeof(App).Assembly)
            }.Show();
        }
    }
}