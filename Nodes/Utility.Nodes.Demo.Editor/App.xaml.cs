using Newtonsoft.Json;
using Splat;
using System.Reflection;
using System.Windows;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Interfaces.NonGeneric.Dependencies;
using Utility.Models.Diagrams;
using Utility.Nodes.Demo.Filters.Services;
using Utility.Nodes.Meta;
using Utility.Repos;
using Utility.ServiceLocation;
using Utility.Services.Meta;
using Utility.WPF.Templates;

namespace Utility.Nodes.Demo.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();


            Locator.CurrentMutable.RegisterConstant<IFilter>(TreeViewFilter.Instance);
            Locator.CurrentMutable.RegisterConstant<IExpander>(WPF.Expander.Instance);
            //Locator.CurrentMutable.RegisterConstant<IContext>(Context.Instance);
            Locator.CurrentMutable.RegisterLazySingleton<MethodCache>(() => MethodCache.Instance);
            Locator.CurrentMutable.RegisterLazySingleton<IObservableIndex<INodeViewModel>>(() => MethodCache.Instance);
            //Locator.CurrentMutable.RegisterLazySingleton<IEnumerableFactory<Method>>(() => NodeMethodFactory.Instance);
            Locator.CurrentMutable.RegisterLazySingleton<IEnumerableFactory<Method>>(() => new NodeMethodFactory());

            //Locator.CurrentMutable.RegisterLazySingleton<MasterViewModel>(() => new MasterViewModel());
            //Locator.CurrentMutable.RegisterLazySingleton<ContainerViewModel>(() => new ContainerViewModel());
            Locator.CurrentMutable.RegisterLazySingleton<System.Windows.Controls.DataTemplateSelector>(() => CustomDataTemplateSelector.Instance);

            initialiseGlobals(Globals.Register);

            JsonConvert.DefaultSettings = () => SettingsFactory.Combined;


            ComboService comboService = new();
            var res = this.FindResource("MasterTemplate") as DataTemplate;
            var window = new Window()
            {/* Content = Locator.Current.GetService<ContainerViewModel>()*/
                ContentTemplate = this.FindResource("MasterTemplate") as DataTemplate
            };

            window.Show();
            MethodCache.Instance[nameof(NodeMethodFactory.BuildContainer)]
                .Subscribe(node =>
                {
                    window.Content = node;

                });


            base.OnStartup(e);

        }


        private static void initialiseGlobals(IRegister register)
        {
            register.Register<IServiceResolver>(() => new ServiceResolver());
            register.Register<INodeSource>(() => new NodeEngine());
            register.Register<ITreeRepository>(() => new TreeRepository("../../../Data"));
            register.Register<IPlaybackEngine>(() => new PlaybackEngine());
        }

    }




}
