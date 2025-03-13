using Jonnidip;
using Newtonsoft.Json;
using Splat;

using System.Windows;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Models.Trees.Converters;
using Utility.Nodes.Demo.Queries.Infrastructure;
using Utility.Nodes.Filters;
using Utility.Repos;

namespace Utility.Nodes.Demo.Queries
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //InitializeComponent();
            
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            Locator.CurrentMutable.RegisterConstant<INodeSource>(NodeEngine.Instance);
            Locator.CurrentMutable.RegisterConstant<IContext>(new Context());
            Locator.CurrentMutable.RegisterConstant<ITreeRepository>(new JsonRepository());
            Locator.CurrentMutable.RegisterConstant<IMainViewModel>(new MainViewModel());
            Locator.CurrentMutable.RegisterLazySingleton<ILiteRepository>(() => new LiteDBRepository(new LiteDBRepository.DatabaseSettings("../../../Data/lite.db", typeof(FilterEntity))));
            Locator.CurrentMutable.RegisterLazySingleton<IJObjectService>(() => new JObjectService());

            var name = typeof(User).Assembly.GetName().Name;
            GlobalModelFilter.Instance.AssemblyPredicate = (a) => a.GetName().Name == name;
            GlobalModelFilter.Instance.TypePredicate = (a) => a == typeof(User);

            JsonConvert.DefaultSettings = () => settings;

            var dataTemplate = App.Current.Resources["MainTemplate"] as DataTemplate;
            var window = new Window() { Content = Locator.Current.GetService<IMainViewModel>() };
            window.Closing += Window_Closing;
            SchemaStore.Instance.Schemas.Add(typeof(FilterEntity), new Schema
            {
                Properties = [
                //new SchemaProperty { Name = nameof(FilterEntity.Key), IsVisible = false },
                //new SchemaProperty { Name = nameof(FilterEntity.GroupKey), IsVisible = false },
                new SchemaProperty { Name = nameof(FilterEntity.Body), IsVisible = false },
                ]
            });
            //window.Loaded += Window_Loaded;
            window.Show();

            base.OnStartup(e);
        }

        private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if(sender is Window { Content:MainViewModel mainViewModel})
            {
                mainViewModel.Save();
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
                //new TypeConverter(),
                new ValueModelConverter(),
                new NodeConverter(),
                new NonGenericPropertyInfoJsonConverter()
                    ]
        };

    }

}
