using Splat;
using System.Configuration;
using System.Data;
using System.Windows;
using Utility.Nodes.Filters;
using Utility.Repos;

namespace Utility.Nodes.Breadcrumbs
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();
            Locator.CurrentMutable.Register<ITreeRepository>(() => TreeRepository.Instance);
            Locator.CurrentMutable.Register<INodeSource>(() => NodeSource.Instance);
            base.OnStartup(e);
        }
    }

}
