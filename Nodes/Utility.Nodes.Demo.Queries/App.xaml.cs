using Jonnidip;
using Newtonsoft.Json;
using Splat;
using System.Windows;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Interfaces.Exs;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Filters;
using Utility.Repos;
using Utility.ViewModels;

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
            Locator.CurrentMutable.RegisterConstant<ITreeRepository>(new Repository());
            Locator.CurrentMutable.RegisterConstant<IMainViewModel>(new MainViewModel());
            Locator.CurrentMutable.Register<ILiteRepository>(() => new LiteDBRepository(new LiteDBRepository.DatabaseSettings("../../../Data/lite.db", typeof(FilterEntity))));

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
                new NodeConverter(),
                new NonGenericPropertyInfoJsonConverter()
                    ]
        };

    }




    public class FilterEntity : NotifyPropertyChangedBase
    {
        private string groupKey;
        private string key;
        private string body;

        public FilterEntity()
        {

        }

        public Guid Id { get; set; } = Guid.NewGuid();

        public string Key
        {
            get => key; set
            {
                if (value == key) return;
                key = value;
                RaisePropertyChanged();
            }
        }

        public string GroupKey
        {
            get => groupKey; set
            {
                if(value == groupKey) return;
                groupKey = value;
                RaisePropertyChanged();
            }
        }

        public string Body
        {
            get => body; set
            {
                if (value == body) return;
                body = value;
                RaisePropertyChanged();
            }
        }
    }

}
