using Utility.PropertyTrees.Abstractions;
using Utility.PropertyTrees.Infrastructure;
using System;
using System.Windows;
using System.Windows.Controls;
using DryIoc;
using Utility.PropertyTrees.WPF.Demo.Infrastructure;
using Utility.GraphShapes;
using Utility.PropertyTrees.Demo.Model;
using System.Reactive.Linq;
using Utility.Observables;

namespace Utility.PropertyTrees.WPF.Demo
{
    public partial class PropertyView : UserControl
    {
        public class Keys
        {
            public const string Model = nameof(PropertyView) + "." + nameof(Model);
            public const string Server = nameof(PropertyView) + "." + nameof(Server);
        }

        private readonly IContainer container;
        private PropertyViewModel viewModel => container.Resolve<PropertyViewModel>();
        private PropertyNode masterNode => container.Resolve<PropertyNode>(Keys.Model);
        private PropertyNode serverNode => container.Resolve<PropertyNode>(Keys.Server);

        public PropertyView(IContainer container)
        {
            this.container = container;

            InitializeComponent();
            masterNode.Data = viewModel.Model;
            serverNode.Data = viewModel.Server;
            ViewModelTree.Engine = container.Resolve<ViewModelEngine>();
            PropertyTree.Engine = new Engine(masterNode);
            ScreensaverSend.Command = viewModel.SendScreensaver;
            LeaderboardSend.Command = viewModel.SendLeaderboard;
            PrizeWheelSend.Command = viewModel.SendPrizewheel;
            ServerConnect.Command = viewModel.Connect;
            viewModel.Subscribe(a =>
            {
                if (a is ViewModelEvent { Name: var name, TreeView: var treeView } clientResponseEvent)
                {
                    var group = new Expander { Header = name, FontSize = 12, Content = treeView, IsExpanded=false };
                    ResponsePanel.Children.Add(group);
                }
            });
        }

        private void PropertyTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is IProperty property)
            {
                ViewModelTree.SelectedObject = property;
            }
        }

        // move to viewmodel
        private void refresh_click(object sender, RoutedEventArgs e)
        {
            viewModel.TreeView(serverNode).Subscribe(treeView =>
            {
                ServerGrid.Children.Clear();
                ServerGrid.Children.Add(treeView);
            });
        }

        private void refresh_2_click(object sender, RoutedEventArgs e)
        {
            {
                PropertyHelper.FindNode(masterNode, a => a is PropertyBase { Name: nameof(Model.ScreenSaver) })
                      .Subscribe(node =>
                      {
                          viewModel.TreeView(node).Subscribe(treeView =>
                          {
                              ScreensaverGrid.Children.Clear();
                              ScreensaverGrid.Children.Add(treeView);
                          });
                      });
            }

            {
                PropertyHelper.FindNode(masterNode, a => a is PropertyBase { Name: nameof(Model.Leaderboard) })
                     .Subscribe(node =>
                     {
                         viewModel.TreeView(node).Subscribe(treeView =>
                         {
                             LeaderboardGrid.Children.Clear();
                             LeaderboardGrid.Children.Add(treeView);
                         });
                     });
            }

            {
                PropertyHelper.FindNode(masterNode, a => a is PropertyBase { Name: nameof(Model.PrizeWheel) })

                    .Subscribe(node =>
                    {
                        viewModel.TreeView(node).Subscribe(treeView =>
                        {
                            PrizeWheelGrid.Children.Clear();
                            PrizeWheelGrid.Children.Add(treeView);
                        });
                    });
            }
        }

        private void show_graph_click(object sender, RoutedEventArgs e)
        {
            new Window { Content = new GraphUserControl(container) }.Show();
            AutoObject.Resolver.Initialise();
        }

        private void initialise_click(object sender, RoutedEventArgs e)
        {
            this.PropertyTree.SelectedObject = viewModel.Model;

        }

        private void show_history_click(object sender, RoutedEventArgs e)
        {
            var controlWindow = new Window { Content = container.Resolve<HistoryViewModel>() };
            ScreenHelper.SetOnFirstScreen(controlWindow);
            controlWindow.Show();
        }
    }

    public record TreeViewRequest(TreeView TreeView, PropertyNode PropertyNode);

    public record TreeViewResponse(TreeView TreeView);
}