using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Splat;
using System.Windows;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes.Demo.Transformers;
using Utility.Nodes.Demo.Filters.Services;
using Utility.Nodes.Filters;
using Utility.Nodes.WPF;
using Utility.Repos;
using Utility.WPF.Templates;
using System.Reflection;
using Utility.Helpers.Reflection;


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

            Locator.CurrentMutable.Register<ITreeRepository>(() => new TreeRepository("../../../Data"));
            //Locator.CurrentMutable.RegisterConstant<INodeSource>(NodeSource.Instance);
            Locator.CurrentMutable.RegisterLazySingleton<INodeSource>(() => new NodeEngine());
            Locator.CurrentMutable.RegisterConstant<IFilter>(TreeViewFilter.Instance);
            Locator.CurrentMutable.RegisterConstant<IExpander>(WPF.Expander.Instance);
            Locator.CurrentMutable.RegisterConstant<IContext>(Context.Instance);
            Locator.CurrentMutable.RegisterConstant<IMethodFactory>(MethodFactory.Instance);
            Locator.CurrentMutable.RegisterConstant<IPropertyFactory>(PropertyFactory.Instance);
            Locator.CurrentMutable.RegisterConstant<Type>(typeof(Node));

            Splat.Locator.CurrentMutable.Register<MethodCache>(() => new MethodCache());
            Splat.Locator.CurrentMutable.RegisterLazySingleton<MainViewModel>(() => new MainViewModel());
            Splat.Locator.CurrentMutable.RegisterLazySingleton<INodePropertyFactory>(() => new NodePropertyFactory());
            Splat.Locator.CurrentMutable.RegisterLazySingleton<System.Windows.Controls.DataTemplateSelector>(() => CustomDataTemplateSelector.Instance);


            //var x = DataTemplateSelector.Instance;

            JsonConvert.DefaultSettings = () => SettingsFactory.Combined;


            TransformerService service = new();
            ControlsService _service = new ControlsService() { Name = "ControlsService" };

            //ComboService comboService = new();

            var window = new Window() { Content = Locator.Current.GetService<MainViewModel>() };

            window.Show();

            base.OnStartup(e);

        }

    }

    public class MethodFactory : IMethodFactory
    {
        Lazy<IEnumerable<MethodInfo>> _methods = new(() => typeof(Methods).StaticMethods().Select(a => a.Item2));
        public IEnumerable<MethodInfo> Methods => _methods.Value;

        public static MethodFactory Instance { get; } = new();
    }


    public class PropertyFactory : IPropertyFactory
    {
        Lazy<IEnumerable<PropertyInfo>> _properties = new(() => [typeof(Node).GetProperty(nameof(Node.Key)), typeof(Node).GetProperty(nameof(Node.Data))]);
        public IEnumerable<PropertyInfo> Properties => _properties.Value;

        public static PropertyFactory Instance { get; } = new();
    }


    public class NodePropertyFactory : INodePropertyFactory
    {
        Lazy<IEnumerable<PropertyInfo>> _properties = new(() => typeof(Node).PublicInstanceProperties().ToArray());

        public IEnumerable<PropertyInfo> Properties(IEnumerable<Type> types) => _properties.Value.Where(a => types.Any(x => a.PropertyType.Equals(x)));
        public static PropertyFactory Instance { get; } = new();

    }

}
