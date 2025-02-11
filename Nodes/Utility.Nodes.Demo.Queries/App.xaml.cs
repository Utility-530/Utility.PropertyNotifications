using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Navigation;

namespace Utility.Nodes.Demo.Queries
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //InitializeComponent();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            var dataTemplate = App.Current.Resources["MainTemplate"] as DataTemplate;
            var window = new Window() { Content = new MainViewModel() };
            //window.Loaded += Window_Loaded;
            window.Show();

            base.OnStartup(e);
        }

    }

}
