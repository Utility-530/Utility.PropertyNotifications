using AdonisUI;
using Newtonsoft.Json;
using Splat;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;
using Utility.Interfaces.NonGeneric.Dependencies;
using Utility.Meta;
using Utility.Models;
using Utility.Models.Diagrams;
using Utility.Nodes.Demo.Lists.Infrastructure;
using Utility.Nodes.Demo.Lists.Services;
using Utility.Nodes.Meta;
using Utility.Repos;
using Utility.ServiceLocation;
using Utility.Services;
using Utility.Services.Meta;
using Utility.WPF.Controls;

namespace Utility.Nodes.Demo.Lists
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string DatabasePath = "O:\\Users\\rytal\\Data\\models.sqlite";

        protected override void OnStartup(StartupEventArgs e)
        {
            CurrentMutable.RegisterLazySingleton(() => new ContainerModel());
            CurrentMutable.RegisterLazySingleton<PlaybackService>(() => new PlaybackService());
            showPlayback();
            SQLitePCL.Batteries.Init();
            initialiseGlobals(Globals.Register);
            initialise(CurrentMutable);
            showSplashscreen();

            JsonConvert.DefaultSettings = () => SettingsFactory.Combined;
            _ = Utility.Nodes.WPF.Templates.SyncFusion.Templates.Instance;
            //subscribeToTypeChanges();
            base.OnStartup(e);
        }

        private static void initialise(IMutableDependencyResolver register)
        {
            register.RegisterConstant<IExpander>(WPF.Expander.Instance);
            register.RegisterLazySingleton<IObservableIndex<INodeViewModel>>(() => MethodCache.Instance);
            register.RegisterLazySingleton<MethodCache>(() => MethodCache.Instance);
            register.RegisterLazySingleton<IEnumerableFactory<Method>>(() => new Factories.NodeMethodFactory());
            register.RegisterLazySingleton<IEnumerableFactory<Method>>(() => Nodes.Meta.NodeMethodFactory.Instance);
            //register.RegisterLazySingleton(() => new MasterViewModel());
            register.RegisterConstant(new ContainerService());
            register.RegisterConstant(new RazorService());
            register.RegisterLazySingleton<IEnumerableFactory<Type>>(() => new ModelTypesFactory());
            register.RegisterLazySingleton<IFactory<EntityMetaData>>(() => new MetaDataFactory());
            register.RegisterLazySingleton<IFactory<IId<Guid>>>(() => new ModelFactory());
            register.RegisterLazySingleton<FilterService>(() => new FilterService());
            register.RegisterLazySingleton<CollectionCreationService>(() => new CollectionCreationService());
            register.RegisterLazySingleton<SelectionService>(() => new SelectionService());
            register.RegisterLazySingleton<CollectionViewService>(() => new CollectionViewService());
            register.RegisterConstant<IFilter>(new StringFilter());
        }

        private static void initialiseGlobals(IRegister register)
        {
            register.Register<IServiceResolver>(() => new ServiceResolver());
            register.Register<INodeSource>(() => new NodeEngine());
            register.Register<ITreeRepository>(() => new TreeRepository("../../../Data"));
            register.Register<IPlaybackEngine>(() => new PlaybackEngine());
        }

        private static void showPlayback()
        {
            //var sswindow = new Window();
            //sswindow.Show();
            //var playBack = new PlayBackUserControl() //{ //};
            //sswindow.Content = playBack;
            //var window = new Window() { Content = Locator.Current.GetService<ContainerViewModel>() };
            //window.Show();
        }

        private static void showSplashscreen()
        {
            var sswindow = new Window();
            sswindow.Show();
            BitmapImage bmi = new(new Uri("pack://application:,,,/Assets/shuttle.png"));
            var slashscreen = new Splashscreen()
            {
                Content = new Image { Source = bmi, Stretch = Stretch.UniformToFill }
            };
            sswindow.Content = slashscreen;
            var window = new Window()
            {
                Content = Locator.Current.GetService<ContainerModel>(),
                ContentTemplate = Utility.WPF.Helpers.ResourceHelper.FindTemplate("MasterTemplate")
            };
            slashscreen.Finished += (s, e) =>
            {
                window.Show();
                sswindow.Close();
            };
        }
    }
}