using System.Windows;
using Newtonsoft.Json;
using Splat;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Models.Trees.Converters;
using Utility.Nodes;
using Utility.Nodify.Base.Abstractions;
using Utility.Nodify.Repository;
using Utility.Nodify.Transitions.Demo.Infrastructure;
using Utility.ServiceLocation;
using MainViewModel = Utility.Nodify.Transitions.Demo.Infrastructure.MainViewModel;

namespace Utility.Nodify.Transitions.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();

            Registration.registerGlobals(Globals.Register);
            initialse();
            JsonConvert.DefaultSettings = () => settings;
            var window = new Window()
            {
                Content = Globals.Resolver.Resolve<MainViewModel>()
            };
            window.Show();
            base.OnStartup(e);
        }

        async void initialse()
        {
            var diagramViewModel = Globals.Resolver.Resolve<IDiagramViewModel>();
            await Globals.Resolver.Resolve<IDiagramFactory>().Build(diagramViewModel);
            var repo = new DiagramRepository("../../../Data");
            repo.Track(diagramViewModel);
            repo.Convert(diagramViewModel);
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
