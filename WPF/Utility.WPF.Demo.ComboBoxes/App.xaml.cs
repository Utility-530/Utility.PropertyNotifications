using Autofac;
using Splat.Autofac;
using System.Configuration;
using System.Data;
using System.Windows;
using Utility.WPF.Controls.Meta;

namespace Utility.WPF.Demo.ComboBoxes
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
                Content = new AssemblyViewControl(typeof(ComboBoxUserControl).Assembly)
            }.Show();
        }
    }

}
