
using Newtonsoft.Json;
using Splat;
using System.Windows;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Models;
using Utility.Models.Trees.Converters;
using Utility.Nodes;
using Utility.Nodes.Meta;
using Utility.Repos;
using Utility.ServiceLocation;
using Utility.Services.Meta;

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

            Globals.Register.Register<ITreeRepository>(TreeRepository.Instance);
            Globals.Register.Register<INodeSource>(NodeEngine.Instance);
            Locator.CurrentMutable.RegisterConstant<IObservableIndex<INodeViewModel>>(MethodCache.Instance);
            Locator.CurrentMutable.RegisterLazySingleton<IEnumerableFactory<Method>>(() => NodeMethodFactory.Instance);


            //Locator.CurrentMutable.RegisterConstant<IFilter>(TreeViewFilter.Instance);
            //Splat.Locator.CurrentMutable.RegisterLazySingleton<MainViewModel>(() => new MainViewModel());

            JsonConvert.DefaultSettings = () => settings;

            var window = new Window
            {
                ContentTemplate = FindResource("ContainerTemplate") as DataTemplate,
            };
            MethodCache
                .Instance[nameof(NodeMethodFactory.BuildCollectionRoot)]
                .Subscribe(a =>
                {
                    window.Content = a;
                });
            window.Show();
            base.OnStartup(e);
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
