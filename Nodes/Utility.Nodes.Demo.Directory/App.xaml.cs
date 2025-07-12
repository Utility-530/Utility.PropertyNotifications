using Newtonsoft.Json;
using Splat;
using System.Windows;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Models;
using Utility.Nodes.Filters;
using Utility.Repos;

namespace Utility.Nodes.Demo.Directory
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
            Locator.CurrentMutable.RegisterConstant<MethodCache>(MethodCache.Instance);
            Locator.CurrentMutable.RegisterConstant<IObservableIndex<INode>>(MethodCache.Instance);
            Locator.CurrentMutable.RegisterLazySingleton<IEnumerableFactory<Method>>(() => NodeMethodFactory.Instance);
            Splat.Locator.CurrentMutable.RegisterLazySingleton<MainViewModel>(() => new MainViewModel());

            JsonConvert.DefaultSettings = () => SettingsFactory.Combined;


            var mainViewModel = Locator.Current.GetService<MainViewModel>();


            base.OnStartup(e);
        }
    }
}
