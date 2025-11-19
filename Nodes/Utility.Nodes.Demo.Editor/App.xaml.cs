using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Splat;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Extensions;
using Utility.Interfaces;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;
using Utility.Interfaces.NonGeneric.Dependencies;
using Utility.Models.Diagrams;
using Utility.Nodes.Demo.Editor.Infrastructure;
using Utility.Nodes.Demo.Editor.Services;
using Utility.Nodes.Demo.Filters.Services;
using Utility.Nodes.Meta;
using Utility.Repos;
using Utility.ServiceLocation;
using Utility.Services.Meta;
using Utility.WPF.Helpers;
using Utility.WPF.Templates;
using Utility.WPF.Trees;
using static Utility.WPF.Controls.Trees.Infrastructure.TreeTabHelper;

namespace Utility.Nodes.Demo.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Subject<AddItemActionCallbackArgs> adds = new();
        private readonly Subject<ItemActionCallbackArgs<TreeViewItem>> removes = new();

        public App()
        {
            Add = callBackArgs =>
            {
                if (callBackArgs.Owner != null)
                {
                    adds.OnNext(callBackArgs);
                    callBackArgs.Cancel();
                }
                else
                    throw new Exception("sd32 22222");
            };

            Remove = callBackArgs =>
            {
                if (callBackArgs.Owner != null)
                {
                    removes.OnNext(callBackArgs);
                    //callBackArgs.Cancel();
                }
                else
                    throw new Exception("sd32 229j22");
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();

            Locator.CurrentMutable.RegisterConstant<IFilter>(TreeViewFilter.Instance);
            Locator.CurrentMutable.RegisterConstant<IExpander>(Utility.WPF.Trees.Expander.Instance);
            //Locator.CurrentMutable.RegisterLazySingleton<MethodCache>(() => MethodCache.Instance);
            Locator.CurrentMutable.RegisterLazySingleton<IObservableIndex<INodeViewModel>>(() => MethodCache.Instance);
            Locator.CurrentMutable.RegisterLazySingleton<IEnumerableFactory<Method>>(() => new NodeMethodFactory());            
            Locator.CurrentMutable.RegisterLazySingleton<IFactory<IId<Guid>>>(() => new ModelFactory());
            Locator.CurrentMutable.RegisterLazySingleton<System.Windows.Controls.DataTemplateSelector>(() => CustomDataTemplateSelector.Instance);

            initialiseGlobals(Globals.Register);
            initialiseConnections(Globals.Resolver.Resolve<IServiceResolver>());

            JsonConvert.DefaultSettings = () => SettingsFactory.Combined;

            ComboService comboService = new();

            var window = new Window()
            {
                ContentTemplate = this.FindResource("ContainerTemplate") as DataTemplate
            };
            var shadowWindow = new Window()
            {
                ContentTemplate = this.FindResource("ShadowTemplate") as DataTemplate
            };
            shadowWindow.ToLeft();
            window.Show();
            shadowWindow.Show();
            MethodCache.Instance[nameof(NodeMethodFactory.BuildContainer)]
                .Subscribe(node =>
                {
                    window.Content = node;
                    shadowWindow.Content = node;

                });

            base.OnStartup(e);
        }

        private static void initialiseGlobals(IRegister register)
        {
            register.Register<IServiceResolver>(() => new ServiceResolver());
            register.Register<INodeSource>(() => new NodeEngine(proliferate: a => (a.Parent() as IGetName)?.Name != NodeMethodFactory.Slave));
            register.Register<ITreeRepository>(() => new TreeRepository("../../../Data"));
            register.Register<IPlaybackEngine>(() => new PlaybackEngine());
        }
        private void initialiseConnections(IServiceResolver register)
        {
            adds.Observe<TabServiceAddItemParam, AddItemActionCallbackArgs>();
            removes.Observe<TabServiceRemoveItemParam, ItemActionCallbackArgs<TreeViewItem>>();
            register.Connect<EngineServiceOutputParam, ComboServiceInputParam>();
            register.Connect<EngineServiceOutputParam, TabServiceAddEngineParam>();
            register.Connect<EngineServiceOutputParam, TabServiceRemoveEngineParam>();
        }

        public AddItemActionCallback Add { get; }

        public RemoveItemActionCallback Remove { get; } 
    }
}
