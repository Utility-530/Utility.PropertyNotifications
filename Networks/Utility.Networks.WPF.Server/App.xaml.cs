using ChatterServer.Services;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Utility.Networks.WPF.Server
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var service = new Service();
            base.OnStartup(e);
        }
    }

}
