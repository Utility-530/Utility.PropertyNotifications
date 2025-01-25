using AutoMapper;
using LiteDB;
using Newtonsoft.Json;
using Splat;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using TinyHtml.Wpf;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Descriptors;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Demo.Filters.Services;
using Utility.Nodes.Filters;
using Utility.Repos;
using Utility.Trees.Demo.Filters;

namespace Utility.Nodes.Demo.Filters
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();

            Locator.CurrentMutable.RegisterConstant<ITreeRepository>(TreeRepository.Instance);
            Locator.CurrentMutable.RegisterConstant<INodeSource>(NodeSource.Instance);
            Locator.CurrentMutable.RegisterConstant<IFilter>(TreeViewFilter.Instance);
            var f = Locator.Current.GetService<IFilter>();
            Splat.Locator.CurrentMutable.RegisterLazySingleton<MainViewModel>(() => new MainViewModel());


            var x = Utility.Trees.Demo.Filters.DataTemplateSelector.Instance;

            JsonConvert.DefaultSettings = NewMethod;

            TransformerService service = new();
            ControlsService _service = new();
            ParserService parserService = new();


            var mainViewModel = Locator.Current.GetService<MainViewModel>();
            //WpfHtmlControlBase.SetMasterStylesheet(combine());
            combine();
            var window = new Window() { Content = mainViewModel };
            window.Loaded += Window_Loaded;
            window.Show();



            base.OnStartup(e);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string serializedXaml = XamlWriter.Save(this.MainWindow);
            Console.WriteLine(serializedXaml);
        }


        private static JsonSerializerSettings NewMethod()
        {
            var _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, CheckAdditionalContent = false };
            foreach (var setting in settings())
            {
                _settings.Converters.Add(setting);
            }
            return _settings;

            static IEnumerable<JsonConverter> settings()
            {
                yield return new AssemblyJsonConverter();
                yield return (new PropertyInfoJsonConverter());
                yield return (new MethodInfoJsonConverter());
                yield return (new ParameterInfoJsonConverter());
                yield return (new AttributeCollectionConverter());
                yield return (new Utility.Conversions.Json.Newtonsoft.DescriptorConverter());
            }
        }

        void combine()
        {
            string name = typeof(App).Assembly.GetManifestResourceNames().Where(a => a.Contains("master.css")).First();

            using (var s = typeof(App).Assembly.GetManifestResourceStream(name))
            {
                WpfHtmlControlBase.SetMasterStylesheet(new StreamReader(s).ReadToEnd());
            }
            //StringBuilder result = new StringBuilder();
            //foreach (var filename in System.IO.Directory.EnumerateFiles("../../../bootstrap-5.3.3-dist/css"))
            //{
            //    using (StreamReader reader = new StreamReader(filename))
            //    {
            //        if (filename.Contains(".css"))
            //        {
            //            result.Append(reader.ReadToEnd());
            //            result.AppendLine();
            //        }                     
            //    }
            //}
            //return result.ToString();            
        }
    }
}
