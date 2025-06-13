using Dragablz;
using Newtonsoft.Json;
using Splat;
using System.Windows;
using Utility.Attributes;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes.Demo.Lists.Infrastructure;
using Utility.Nodes.Demo.Lists.Services;
using Utility.Nodes.Filters;
using Utility.Nodes.WPF;
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
            Locator.CurrentMutable.RegisterConstant<IFilter>(TreeViewFilter.Instance);
            Locator.CurrentMutable.RegisterConstant<IExpander>(WPF.Expander.Instance);
            //Locator.CurrentMutable.RegisterConstant<IContext>(Globals);
            Locator.CurrentMutable.RegisterLazySingleton<MethodCache>(() => new MethodCache());
            Locator.CurrentMutable.RegisterLazySingleton<INodeMethodFactory>(() => new Utility.Nodes.Demo.Lists.Services.NodeMethodFactory());
            Locator.CurrentMutable.RegisterLazySingleton<INodeMethodFactory>(() => new Nodes.Filters.NodeMethodFactory());
            Locator.CurrentMutable.RegisterLazySingleton(() => new MasterViewModel());
            Locator.CurrentMutable.RegisterLazySingleton(() => new ContainerViewModel());
            Locator.CurrentMutable.RegisterLazySingleton<System.Windows.Controls.DataTemplateSelector>(() => CustomDataTemplateSelector.Instance);

            Locator.CurrentMutable.RegisterConstant<IObservable<ViewModel>>(new ComboService());
            Locator.CurrentMutable.RegisterConstant(new ContainerService());
            Locator.CurrentMutable.RegisterConstant(new RazorService());
            Locator.CurrentMutable.RegisterLazySingleton<IModelTypesFactory>(() => new ModelTypesFactory());

            JsonConvert.DefaultSettings = () => SettingsFactory.Combined;

            //TransformerService service = new();
            //ControlsService _service = ControlsService.Instance;
            //ComboService comboService = new ();
            Utility.Models.SchemaStore.Instance.Add(typeof(EbayModel), SchemaFactory.EbaySchema);
            var window = new Window() { Content = Locator.Current.GetService<ContainerViewModel>() };

            window.Show();

            base.OnStartup(e);

        }

    }

    public class ModelTypesFactory : IModelTypesFactory
    {
        public IEnumerable<Type> Types()
        {
            return typeof(ModelTypesFactory).Assembly.TypesByAttribute<ModelAttribute>();
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
