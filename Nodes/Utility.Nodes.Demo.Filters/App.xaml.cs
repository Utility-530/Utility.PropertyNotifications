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
using Utility.Nodes.Demo.Filters.Services;
using Utility.Nodes.Filters;
using Utility.Repos;
using N = Utility.Nodes.Filters.Node;

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
            Splat.Locator.CurrentMutable.RegisterLazySingleton<MainViewModel>(() => new MainViewModel());


            var x = Utility.Trees.Demo.Filters.DataTemplateSelector.Instance;

            JsonConvert.DefaultSettings = NewMethod;
            Splat.Locator.CurrentMutable.RegisterLazySingleton(() =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<N, NodeDTO>()
                    .ForMember(dest => dest.CurrentGuid, opt => opt.MapFrom(a => Guid.Parse(a.Current.Key)))
                            .ForMember(dest => dest.Index, opt => opt.MapFrom(a => a.LocalIndex))
                    .AfterMap((src, dest) =>
                    {
                        try
                        {
                            dest.Data = JsonConvert.SerializeObject(src.Data);
                        }
                        catch (Exception ex)
                        {

                        }
                        convertBackFromPersistable(dest.Data);
                    });

                    cfg.CreateMap<NodeDTO, N>()
                        .ForMember(dest => dest.LocalIndex, opt => opt.MapFrom(a => a.Index))
                            .AfterMap((src, dest) =>
                            {
                                dest.Data = convertBackFromPersistable(src.Data);

                                if (src.CurrentGuid != default)
                                    NodeSource.Instance.FindNodeAsync(src.CurrentGuid).Subscribe(a =>
                                    {
                                        a.Parent = dest;
                                        dest.Current = a;
                                    });
                            });
                });

                var mapper = config.CreateMapper();
                return mapper;
            });


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



        object convertBackFromPersistable(string data)
        {

            return JsonConvert.DeserializeObject(data);

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
