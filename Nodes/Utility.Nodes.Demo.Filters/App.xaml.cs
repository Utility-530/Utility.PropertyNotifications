using Newtonsoft.Json;
using Splat;
using System.Windows;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes.Filters;
using Utility.Repos;

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
            Locator.CurrentMutable.RegisterConstant<INodeSource>(NodeEngine.Instance);
            Locator.CurrentMutable.RegisterConstant<IContext>(Context.Instance);
            //Locator.CurrentMutable.RegisterConstant<IFilter>(TreeViewFilter.Instance);
            //Splat.Locator.CurrentMutable.RegisterLazySingleton<MainViewModel>(() => new MainViewModel());

            JsonConvert.DefaultSettings = () => SettingsFactory.Combined;

            base.OnStartup(e);
        }
    }

}
