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
            //Locator.CurrentMutable.RegisterConstant<INodeSource>(NodeSource.Instance);
            Locator.CurrentMutable.RegisterConstant<INodeSource>(NodeEngine.Instance);
            Locator.CurrentMutable.RegisterConstant<MethodCache>(new MethodCache());
            //Locator.CurrentMutable.RegisterConstant<IFilter>(TreeViewFilter.Instance);
            //Locator.CurrentMutable.RegisterConstant<IExpander>(Expander.Instance);
            Locator.CurrentMutable.RegisterConstant<IContext>(Context.Instance);
            Splat.Locator.CurrentMutable.RegisterLazySingleton<MainViewModel>(() => new MainViewModel());
            //Splat.Locator.CurrentMutable.RegisterLazySingleton<System.Windows.Controls.DataTemplateSelector>(() => CustomDataTemplateSelector.Instance);


            //var x = DataTemplateSelector.Instance;

            JsonConvert.DefaultSettings = () => SettingsFactory.Combined;

            //TransformerService service = new();
            //ControlsService _service = ControlsService.Instance;
            //ParserService parserService = new();


            var mainViewModel = Locator.Current.GetService<MainViewModel>();

            //var window = new Window() { Content = mainViewModel };

            //window.Show();



            base.OnStartup(e);
        }
    }
}
