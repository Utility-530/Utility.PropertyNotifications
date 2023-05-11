using Utility.PropertyTrees.Abstractions;
using Utility.PropertyTrees.Infrastructure;
using System;
using System.Windows;
using System.Windows.Controls;
using DryIoc;
using Utility.PropertyTrees.WPF.Demo.Infrastructure;
using Utility.Graph.Shapes;
using Utility.PropertyTrees.Demo.Model;
using System.Reactive.Linq;
using Utility.Observables;
using Utility.PropertyTrees.WPF.Demo.Views;
using System.Reactive.Threading.Tasks;
using System.Threading;

namespace Utility.PropertyTrees.WPF.Demo
{
    public partial class MainView : UserControl
    {
        public class Keys
        {
            public const string Model = nameof(MainView) + "." + nameof(Model);
            public const string Server = nameof(MainView) + "." + nameof(Server);
        }

        private readonly IContainer container;
        private MainViewModel viewModel => container.Resolve<MainViewModel>();
        private PropertyNode masterNode => container.Resolve<PropertyNode>(Keys.Model);
        private PropertyNode serverNode => container.Resolve<PropertyNode>(Keys.Server);
        private ViewModelEngine viewModelEngine => container.Resolve<ViewModelEngine>();
        private SynchronizationContext context => container.Resolve<SynchronizationContext>();

        public MainView(IContainer container)
        {
            this.container = container;

            InitializeComponent();
            masterNode.Data = viewModel.Model;
            serverNode.Data = viewModel.Server;

            //ViewModelTree.Engine = container.Resolve<ViewModelEngine>();
            //PropertyTree.Engine = new Engine(masterNode);
            ScreensaverSend.Command = viewModel.SendScreensaver;
            LeaderboardSend.Command = viewModel.SendLeaderboard;
            PrizeWheelSend.Command = viewModel.SendPrizewheel;
            ServerConnect.Command = viewModel.Connect;
            viewModel.Subscribe(a =>
            {
                if (a is ViewModelEvent { Name: var name, TreeView: var treeView } clientResponseEvent)
                {
                    var group = new Expander { Header = name, FontSize = 12, Content = treeView, IsExpanded = false };
                    ResponsePanel.Children.Add(group);
                }
            });
        }

        private void PropertyTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem { Header: PropertyNode propertyNode })
            {
                viewModelEngine
                    .Convert(propertyNode)
                    .ToObservable()
                    .Cast<PropertyNode>()
                     .Subscribe(node =>
                     {
                         context.Post((a) =>
                         {
                             ViewModelTree.Items.Clear();
                             viewModel.TreeView(node, ViewModelTree)
                                 .Subscribe(treeView =>
                                 {
                                 });
                         }, default);                     
                     });
                //ViewModelTree.SelectedObject = property;
            }
            else
                throw new Exception("dfgf 543432eee");
        }

        // move to viewmodel
        private void refresh_click(object sender, RoutedEventArgs e)
        {
            viewModel.TreeView(serverNode, ServerGrid).Subscribe(treeView =>
            {
            });
        }

        private void refresh_2_click(object sender, RoutedEventArgs e)
        {
            {
                PropertyHelper.FindNode(masterNode, a => a is PropertyBase { Name: nameof(Model.ScreenSaver) })
                      .Subscribe(node =>
                      {
                          viewModel.TreeView(node, ScreensaverGrid).Subscribe(treeView =>
                          {     
                          });
                      });
            }

            {
                PropertyHelper.FindNode(masterNode, a => a is PropertyBase { Name: nameof(Model.Leaderboard) })
                     .Subscribe(node =>
                     {
                         viewModel.TreeView(node, LeaderboardGrid).Subscribe(treeView =>
                         {
                         });
                     });
            }

            {
                PropertyHelper.FindNode(masterNode, a => a is PropertyBase { Name: nameof(Model.PrizeWheel) })

                    .Subscribe(node =>
                    {
                        viewModel.TreeView(node, PrizeWheelGrid).Subscribe(treeView =>
                        {
                        });
                    });
            }
        }

        private void show_graph_click(object sender, RoutedEventArgs e)
        {
            new Window { Content = new GraphUserControl(container) }.Show();
            //AutoObject.Resolver.Initialise();
        }

        private void initialise_click(object sender, RoutedEventArgs e)
        {
            //this.PropertyTree.SelectedObject = viewModel.Model;
            viewModel.TreeView(masterNode, PropertyTree).Subscribe(treeView =>
            {
                //PropertyTree.Children.Clear();
                //PropertyTree.Children.Add(treeView);
            });

        }

        private void show_history_click(object sender, RoutedEventArgs e)
        {
            var controlWindow = new Window { Content = container.Resolve<HistoryViewModel>() };
            ScreenHelper.SetOnFirstScreen(controlWindow);
            controlWindow.Show();
        }

        private void show_templates_click(object sender, RoutedEventArgs e)
        {
            new Window { Content = new TemplatesView(container) }.Show();
        }
    }


}