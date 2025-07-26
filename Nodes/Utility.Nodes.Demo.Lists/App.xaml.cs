using Dragablz;
using Newtonsoft.Json;
using Splat;
using System;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utility.Attributes;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Extensions;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Demo.Lists.Entities;
using Utility.Nodes.Demo.Lists.Infrastructure;
using Utility.Nodes.Demo.Lists.Services;
using Utility.Nodes.Filters;
using Utility.PropertyNotifications;
using Utility.Repos;
using Utility.Services;
using Utility.WPF.Controls;
using Utility.WPF.Templates;
using Utility.ServiceLocation;
using Utility.Interfaces.NonGeneric.Dependencies;

namespace Utility.Nodes.Demo.Lists
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            CurrentMutable.RegisterLazySingleton(() => new ContainerViewModel());
            CurrentMutable.RegisterLazySingleton<PlaybackService>(() => new PlaybackService());
            showPlayback();
            SQLitePCL.Batteries.Init();
            initialise(CurrentMutable);
            initialiseGlobals(Globals.Register);
            showSplashscreen();
            buildNetwork(Globals.Resolver.Resolve<IServiceResolver>());
            JsonConvert.DefaultSettings = () => SettingsFactory.Combined;
            subscribeToTypeChanges();
            base.OnStartup(e);
        }

        private static void initialise(IMutableDependencyResolver register)
        {
            register.RegisterConstant<IExpander>(WPF.Expander.Instance);
            register.RegisterLazySingleton<IObservableIndex<INode>>(() => MethodCache.Instance);
            register.RegisterLazySingleton<MethodCache>(() => MethodCache.Instance);
            register.RegisterLazySingleton<IEnumerableFactory<Method>>(() => new Factories.NodeMethodFactory());
            register.RegisterLazySingleton<IEnumerableFactory<Method>>(() => Nodes.Filters.NodeMethodFactory.Instance);
            register.RegisterLazySingleton(() => new MasterViewModel());
            register.RegisterConstant(new ContainerService());
 
            register.RegisterConstant(new RazorService());
            register.RegisterLazySingleton<IEnumerableFactory<Type>>(() => new ModelTypesFactory());
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

        private static void buildNetwork(IServiceResolver serviceResolver)
        {
            serviceResolver.Connect<PredicateReturnParam, PredicateParam>();
            serviceResolver.Connect<ListInstanceReturnParam, ListInParam>();
            serviceResolver.Connect<ListInstanceReturnParam, ListParam>();
        }

        private static void showPlayback()
        {
            //var sswindow = new Window();
            //sswindow.Show();
            //var playBack = new PlayBackUserControl()
            //{
            //};
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
            var window = new Window() { Content = Locator.Current.GetService<ContainerViewModel>() };
            slashscreen.Finished += (s, e) =>
            {
                window.Show();
                sswindow.Close();
            };
        }

        public void subscribeToTypeChanges()
        {
            Locator.Current.GetService<IObservableIndex<INode>>()[nameof(Utility.Nodes.Filters.NodeMethodFactory.BuildListRoot)]
                .Subscribe(node =>
                {
                    node
                    .WithChangesTo(a => a.Current)
                    .Select(a =>
                    {
                        if (a.Data is ModelTypeModel { Value.Type: { } stype } data)
                        {
                            var type = Type.GetType(stype);
                            return type;
                        }
                        throw new Exception("£D£");
                    }).Observe<ChangeTypeParam, Type>()
                    .Observe<InstanceTypeParam, Type>();
                });
        }
    }

    public class ModelTypesFactory : IEnumerableFactory<Type>
    {
        public IEnumerable<Type> Create(object? o = null)
        {
            return typeof(ModelTypesFactory).Assembly.TypesByAttribute<ModelAttribute>();
        }
    }

    public class ModelFactory : IFactory<IId<Guid>>
    {
        public IId<Guid> Create(object config)
        {
            if (config is Type type)
            {
                if (type.GetConstructors().SingleOrDefault(a => a.TryGetAttribute<FactoryAttribute>(out var x)) is { } x)
                {
                    return (IId<Guid>)x.Invoke(new[] { default(object) });
                }
                if (type == typeof(UserProfileModel))
                {
                    return new UserProfileModel() { Id = Guid.NewGuid(), AddDate = DateTime.Now };
                }
                if (type == typeof(EbayModel))
                {
                    return new EbayModel { Id = Guid.NewGuid() };
                }
                if (Activator.CreateInstance(type) is IId<Guid> iid)
                {
                    if (iid is IIdSet<Guid> set)
                        set.Id = Guid.NewGuid();
                    return iid;
                }
                else
                {
                    throw new Exception("33889__e");
                }
            }
            else
            {
                throw new Exception("545 fgfgddf");
            }
        }
    }


    public class InterTabClient : IInterTabClient
    {
        public InterTabClient()
        {
        }

        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var window = new Window();
            var TabablzControl = new TabablzControl();
            window.Content = TabablzControl;

            return new NewTabHost<Window>(window, TabablzControl);
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            return TabEmptiedResponse.CloseWindowOrLayoutBranch;
        }
    }




    public class PlaybackEngine : IPlaybackEngine
    {
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(IAction value)
        {
            value.Do();
        }
    }


}
