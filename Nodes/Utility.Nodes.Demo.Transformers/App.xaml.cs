using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Splat;
using System.Reflection;
using System.Windows;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Demo.Filters.Services;
using Utility.Nodes.Demo.Transformers;
using Utility.Nodes.Filters;
using Utility.Nodes.WPF;
using Utility.Repos;
using Utility.WPF.Templates;
using Utility.ServiceLocation;


namespace Utility.Nodes.Demo.Transformers
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();

            registerGlobals();

            Locator.CurrentMutable.RegisterConstant<IFilter>(TreeViewFilter.Instance);
            Locator.CurrentMutable.RegisterConstant<IExpander>(WPF.Expander.Instance);

            Locator.CurrentMutable.RegisterConstant<IEnumerableFactory<MethodInfo>>(MethodFactory.Instance);
            Locator.CurrentMutable.RegisterConstant<IEnumerableFactory<PropertyInfo>>(PropertyFactory.Instance);


            Locator.CurrentMutable.RegisterConstant<Type>(typeof(Node));

            Locator.CurrentMutable.Register<IObservableIndex<INode>>(() => MethodCache.Instance);
            Locator.CurrentMutable.RegisterLazySingleton<IEnumerableFactory<Method>>(() =>  Nodes.Filters.NodeMethodFactory.Instance);
            Locator.CurrentMutable.RegisterLazySingleton<MainViewModel>(() => new MainViewModel());
            Locator.CurrentMutable.Register<InputSelectionViewModel>(() => new InputSelectionViewModel());
            Locator.CurrentMutable.RegisterLazySingleton<System.Windows.Controls.DataTemplateSelector>(() => CustomDataTemplateSelector.Instance);

            JsonConvert.DefaultSettings = () => SettingsFactory.Combined;

            Locator.CurrentMutable.RegisterConstant<ControlsService>(new() { Name = "ControlsService" });
            Locator.CurrentMutable.RegisterConstant<IObservable<ControlEvent>>(new InputControlsService() { Name = "ControlsService" });

            var window = new Window() { Content = Locator.Current.GetService<MainViewModel>() };

            window.Show();

            base.OnStartup(e);
        }

        void registerGlobals()
        {
            Globals.Register.Register<ITreeRepository>(() => new TreeRepository("../../../Data"));
            Globals.Register.Register<INodeSource>(() => new NodeEngine());
        }
    }

    public class MethodFactory : IEnumerableFactory<MethodInfo>
    {
        Lazy<IEnumerable<MethodInfo>> _methods = new(() => typeof(Methods).StaticMethods().Select(a => a.Item2));

        public static MethodFactory Instance { get; } = new();

        public IEnumerable<MethodInfo> Create(object config)
        {
            if (config is nameof(MethodsModel))
                return _methods.Value;
            throw new Exception("ds 3333");

        }
    }


    public class PropertyFactory : IEnumerableFactory<PropertyInfo>
    {
        Lazy<IEnumerable<PropertyInfo>> _properties = new(() => [typeof(Node).GetProperty(nameof(Node.Key)), typeof(Node).GetProperty(nameof(Node.Data))]);

        public static PropertyFactory Instance { get; } = new();

        public IEnumerable<PropertyInfo> Create(object config)
        {
            if (config is IEnumerable<Type> types)
                return _properties.Value.Where(a => types.Any(x => a.PropertyType.Equals(x)));
            return _properties.Value;
        }
    }
}
