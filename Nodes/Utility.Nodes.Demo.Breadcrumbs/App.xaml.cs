using Jonnidip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Splat;
using System.Windows;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Interfaces.Exs;
using Utility.Nodes.Demo.Queries.Infrastructure;
using Utility.Nodes.Filters;
using Utility.Repos;

namespace Utility.Nodes.Breadcrumbs
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        INode node;

        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();
            Locator.CurrentMutable.RegisterLazySingleton<ITreeRepository>(() => JsonRepository.Instance);
            Locator.CurrentMutable.RegisterLazySingleton<INodeSource>(() => NodeEngine.Instance);
            Locator.CurrentMutable.RegisterLazySingleton<Type>(() => typeof(Node));
            Locator.CurrentMutable.RegisterLazySingleton<IJObjectService>(() => new JObjectService());

            JsonConvert.DefaultSettings = () => settings;

            find();
            var winow = new Window { Content = node, ContentTemplate = this.Resources["main"] as DataTemplate };
            winow.Show();
            base.OnStartup(e);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var obj = JObject.FromObject(this.node as Node);
            Locator.Current.GetService<IJObjectService>().Set(obj);

        }

        private void find()
        {

            if (Locator.Current.GetService<IJObjectService>().Get() is not JObject jObject)
            {
                Locator.Current.GetService<INodeSource>()
                   .Single(nameof(NodeMethodFactory.BreadcrumbRoot))
                   .Subscribe(node =>
                   {
                       this.node = node;
                   });
            }
            else
            {
                var node = jObject.ToObject<Node>();
                Locator.Current.GetService<INodeSource>().Add(node);
                this.node = node;
            }
        }



        public JsonSerializerSettings settings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            //Formatting = Formatting.Indented,
            Converters = [
                new AssemblyJsonConverter(),
                new PropertyInfoJsonConverter(),
                new MethodInfoJsonConverter(),
                new ParameterInfoJsonConverter(),
                new AttributeCollectionConverter(),
                new DescriptorConverter(),
                new StringTypeEnumConverter(),
                new NodeConverter(),
                new NonGenericPropertyInfoJsonConverter()
    ]
        };
    }

}
