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
using Utility.WPF.Demo.Buttons;
using Utility.WPF.Templates;

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
            CurrentMutable.RegisterConstant<IPlaybackEngine>(new PlayBackViewModel());
            CurrentMutable.RegisterLazySingleton<PlaybackService>(() => new PlaybackService());

            showPlayback();
            //showSplashscreen();
            SQLitePCL.Batteries.Init();

            CurrentMutable.Register<ITreeRepository>(() => new TreeRepository("../../../Data"));
            CurrentMutable.RegisterLazySingleton<INodeSource>(() => new NodeEngine());

            CurrentMutable.RegisterConstant<IExpander>(WPF.Expander.Instance);
            CurrentMutable.RegisterLazySingleton<IObservableIndex<INode>>(() => MethodCache.Instance);
            CurrentMutable.RegisterLazySingleton<MethodCache>(() => MethodCache.Instance);
            CurrentMutable.RegisterLazySingleton<IEnumerableFactory<Method>>(() => new Services.NodeMethodFactory());
            CurrentMutable.RegisterLazySingleton<IEnumerableFactory<Method>>(() => Nodes.Filters.NodeMethodFactory.Instance);
            CurrentMutable.RegisterLazySingleton(() => new MasterViewModel());

            CurrentMutable.RegisterLazySingleton<DataTemplateSelector>(() => CustomDataTemplateSelector.Instance);

            //Locator.CurrentMutable.RegisterConstant<System.IObservable<ViewModel>>(new ComboService());
            CurrentMutable.RegisterConstant(new ContainerService());
            CurrentMutable.RegisterConstant(new ServiceResolver());
            CurrentMutable.RegisterConstant(new RazorService());
            CurrentMutable.RegisterLazySingleton<IEnumerableFactory<Type>>(() => new ModelTypesFactory());
            CurrentMutable.RegisterLazySingleton<IFactory<IId<Guid>>>(() => new ModelFactory());

            CurrentMutable.RegisterLazySingleton<FilterService>(() => new FilterService());
            CurrentMutable.RegisterLazySingleton<CollectionCreationService>(() => new CollectionCreationService());
            CurrentMutable.RegisterLazySingleton<SelectionService>(() => new SelectionService());
            CurrentMutable.RegisterLazySingleton<CollectionViewService>(() => new CollectionViewService());
            CurrentMutable.RegisterConstant<IFilter>(new StringFilter());



            Locator.Current.GetService<ServiceResolver>().Connect<PredicateReturnParam, PredicateParam>();
            //Locator.Current.GetService<ServiceResolver>().Connect<ListCollectionViewReturnParam, ListCollectionViewParam>();
            Locator.Current.GetService<ServiceResolver>().Connect<ListInstanceReturnParam, ListInParam>();
            Locator.Current.GetService<ServiceResolver>().Connect<ListInstanceReturnParam, ListParam>();

            JsonConvert.DefaultSettings = () => SettingsFactory.Combined;
            subscribeToTypeChanges();
            base.OnStartup(e);
        }

        private static void showPlayback()
        {
            var sswindow = new Window();
            sswindow.Show();
            var playBack = new PlayBackUserControl()
            {
            };
            sswindow.Content = playBack;
            var window = new Window() { Content = Locator.Current.GetService<ContainerViewModel>() };

            window.Show();

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

    //public class EbayModelFactory : IFactory<EbayModel>
    //{
    //    public Task<EbayModel> Create(object config)
    //    {
    //        return Task.FromResult(new EbayModel() { Id = Guid.NewGuid() });
    //    }
    //}
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


}
