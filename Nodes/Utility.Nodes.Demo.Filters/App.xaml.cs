using Newtonsoft.Json;
using Splat;
using System.Configuration;
using System.Data;
using System.Windows;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes.Filters;

//using Utility.Nodes.Filters;
using Utility.Repos;
using Utility.Trees.Demo.Filters.Infrastructure;

namespace Utility.Trees.Demo.Filters
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();

            Locator.CurrentMutable.RegisterConstant<ITreeRepository>(TreeRepository.Instance);
            Locator.CurrentMutable.RegisterConstant<INodeSource>(NodeSource.Instance);
            Locator.CurrentMutable.RegisterConstant<IContext>(Context.Instance);
            //Locator.CurrentMutable.RegisterConstant<IFilter>(TreeViewFilter.Instance);
            //Splat.Locator.CurrentMutable.RegisterLazySingleton<MainViewModel>(() => new MainViewModel());

            JsonConvert.DefaultSettings = () => SettingsFactory.Combined;

            base.OnStartup(e);
        }
    }

}
