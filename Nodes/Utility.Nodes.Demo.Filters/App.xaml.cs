using AutoMapper;
using DryIoc.ImTools;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Splat;
using System.Configuration;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Nodes.Demo.Filters.Infrastructure;
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
                    .ForMember(dest => dest.CurrentGuid, opt => opt.MapFrom(a => a.Current.Guid))
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


            Service service = new();
            var _service = new ControlsService();


            var mainViewModel = Locator.Current.GetService<MainViewModel>();

            var window = new Window() { Content = mainViewModel };
            window.Show();

            base.OnStartup(e);
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
    }
}
