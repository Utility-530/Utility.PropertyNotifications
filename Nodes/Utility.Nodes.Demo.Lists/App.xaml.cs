using Dragablz;
using Newtonsoft.Json;
using Splat;
using System;
using System.Reflection;
using System.Windows;
using Utility.Attributes;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Demo.Lists.Entities;
using Utility.Nodes.Demo.Lists.Infrastructure;
using Utility.Nodes.Demo.Lists.Services;
using Utility.Nodes.Filters;
using Utility.Repos;
using Utility.Services;
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
            SQLitePCL.Batteries.Init();

            Locator.CurrentMutable.Register<ITreeRepository>(() => new TreeRepository("../../../Data"));
            Locator.CurrentMutable.RegisterLazySingleton<INodeSource>(() => new NodeEngine());
  
            Locator.CurrentMutable.RegisterConstant<IExpander>(WPF.Expander.Instance);
            //Locator.CurrentMutable.RegisterConstant<IContext>(Globals);
            Locator.CurrentMutable.RegisterLazySingleton<MethodCache>(() => new MethodCache());
            Locator.CurrentMutable.RegisterLazySingleton<IEnumerableFactory<MethodInfo>>(() => new Services.NodeMethodFactory());
            Locator.CurrentMutable.RegisterLazySingleton<IEnumerableFactory<MethodInfo>>(() => new Nodes.Filters.NodeMethodFactory());
            Locator.CurrentMutable.RegisterLazySingleton(() => new MasterViewModel());
            Locator.CurrentMutable.RegisterLazySingleton(() => new ContainerViewModel());
            Locator.CurrentMutable.RegisterLazySingleton<System.Windows.Controls.DataTemplateSelector>(() => CustomDataTemplateSelector.Instance);

            //Locator.CurrentMutable.RegisterConstant<System.IObservable<ViewModel>>(new ComboService());
            Locator.CurrentMutable.RegisterConstant(new ContainerService());
            Locator.CurrentMutable.RegisterConstant(new RazorService());
            Locator.CurrentMutable.RegisterLazySingleton<IEnumerableFactory<Type>>(() => new ModelTypesFactory());
            Locator.CurrentMutable.RegisterLazySingleton<Utility.Interfaces.Generic.IFactory<IId<Guid>>>(() => new ModelFactory());

            Locator.CurrentMutable.RegisterLazySingleton<FilterService>(() => new FilterService());
            Locator.CurrentMutable.RegisterLazySingleton<CollectionCreationService>(() => new CollectionCreationService());
            Locator.CurrentMutable.RegisterLazySingleton<SelectionService>(() => new SelectionService());
            Locator.CurrentMutable.RegisterLazySingleton<CollectionViewService>(() => new CollectionViewService());
            Locator.CurrentMutable.RegisterConstant<IFilter>(new StringFilter());

            Locator.Current.GetService<CollectionCreationService>().Subscribe(Locator.Current.GetService<FilterService>());
            Locator.Current.GetService<FilterService>().Subscribe(Locator.Current.GetService<CollectionViewService>());
            Locator.Current.GetService<CollectionCreationService>().Subscribe(Locator.Current.GetService<CollectionViewService>());

            JsonConvert.DefaultSettings = () => SettingsFactory.Combined;

            //TransformerService service = new();
            //ControlsService _service = ControlsService.Instance;
            //ComboService comboService = new ();
            //Utility.Models.SchemaStore.Instance.Add(typeof(EbayModel), SchemaFactory.EbaySchema);
            var window = new Window() { Content = Locator.Current.GetService<ContainerViewModel>() };

            window.Show();

            base.OnStartup(e);

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
    public class ModelFactory : Utility.Interfaces.Generic.IFactory<IId<Guid>>
    {
        public IId<Guid> Create(object config)
        {
            if (config is Type type)
            {
                if (type == typeof(UserProfileModel))
                {
                    return (IId<Guid>)new UserProfileModel() { Id = Guid.NewGuid(), AddDate = DateTime.Now };
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
