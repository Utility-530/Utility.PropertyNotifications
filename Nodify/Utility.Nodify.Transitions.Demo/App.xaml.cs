using DryIoc;

using Newtonsoft.Json;
using Splat;
using System.Windows;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Models.Trees.Converters;
using Utility.Nodes;
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
            //Registration.initialise(Locator.CurrentMutable);
            JsonConvert.DefaultSettings = () => settings;
            var window = new Window()
            {
                Content = Globals.Resolver.Resolve<MainViewModel>()
            };
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
