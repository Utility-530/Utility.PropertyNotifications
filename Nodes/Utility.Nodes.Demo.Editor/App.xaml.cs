using Newtonsoft.Json;
using Splat;
using System.Reflection;
using System.Windows;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes.Demo.Filters.Services;
using Utility.Nodes.Filters;
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
            //Locator.CurrentMutable.RegisterConstant<IContext>(Context.Instance);
            Locator.CurrentMutable.RegisterLazySingleton<MethodCache>(() => MethodCache.Instance);
            Locator.CurrentMutable.RegisterLazySingleton<IObservableIndex<INode>>(() => MethodCache.Instance);
            Locator.CurrentMutable.RegisterLazySingleton<IEnumerableFactory<Method>>(() => NodeMethodFactory.Instance);

            Locator.CurrentMutable.RegisterLazySingleton<MasterViewModel>(() => new MasterViewModel());
            Locator.CurrentMutable.RegisterLazySingleton<ContainerViewModel>(() => new ContainerViewModel());
            Locator.CurrentMutable.RegisterLazySingleton<System.Windows.Controls.DataTemplateSelector>(() => CustomDataTemplateSelector.Instance);


            JsonConvert.DefaultSettings = () => SettingsFactory.Combined;

            ControlsService _service = ControlsService.Instance;

            ComboService comboService = new ();

            var window = new Window() { Content = Locator.Current.GetService<ContainerViewModel>() };

            window.Show();

            base.OnStartup(e);

        }

    }
}
