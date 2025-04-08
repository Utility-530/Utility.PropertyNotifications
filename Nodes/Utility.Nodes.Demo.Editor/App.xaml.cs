using Newtonsoft.Json;
using Splat;
using System.Windows;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes.Demo.Filters.Services;
using Utility.Nodes.Filters;
using Utility.Nodes.WPF;
using Utility.Repos;
using Utility.WPF.Templates;


namespace Utility.Nodes.Demo.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();

            Locator.CurrentMutable.Register<ITreeRepository>(()=> new TreeRepository("../../../Data"));
            //Locator.CurrentMutable.RegisterConstant<INodeSource>(NodeSource.Instance);
            Locator.CurrentMutable.RegisterLazySingleton<INodeSource>(()=>new NodeEngine());
            Locator.CurrentMutable.RegisterConstant<IFilter>(TreeViewFilter.Instance);
            Locator.CurrentMutable.RegisterConstant<IExpander>(WPF.Expander.Instance);
            Locator.CurrentMutable.RegisterConstant<IContext>(Context.Instance);
       
            Splat.Locator.CurrentMutable.Register<MethodCache>(() => new MethodCache());
            Splat.Locator.CurrentMutable.RegisterLazySingleton<MasterViewModel>(() => new MasterViewModel());
            Splat.Locator.CurrentMutable.RegisterLazySingleton<ContainerViewModel>(() => new ContainerViewModel());
            Splat.Locator.CurrentMutable.RegisterLazySingleton<System.Windows.Controls.DataTemplateSelector>(() => CustomDataTemplateSelector.Instance);


            //var x = DataTemplateSelector.Instance;

            JsonConvert.DefaultSettings = () => SettingsFactory.Combined;

            //TransformerService service = new();
            ControlsService _service = ControlsService.Instance;

            ComboService comboService = new ();

            var window = new Window() { Content = Locator.Current.GetService<ContainerViewModel>() };

            window.Show();

            base.OnStartup(e);

        }

    }
}
