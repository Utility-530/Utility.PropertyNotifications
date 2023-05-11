using DryIoc;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using Utility.PropertyTrees.Demo.Model;
using Utility.PropertyTrees.Infrastructure;
using static Utility.PropertyTrees.WPF.Demo.LightBootStrapper;
using Utility.Observables;

namespace Utility.PropertyTrees.WPF.Demo.Views
{

    /// <summary>
    /// Interaction logic for PropertyView.xaml
    /// </summary>
    public partial class PropertyView : UserControl
    {
        public class Keys
        {
            public const string Model = nameof(MainView) + "." + nameof(Model);
            public const string Server = nameof(MainView) + "." + nameof(Server);
        }

        private PropertyNode masterNode => container.Resolve<PropertyNode>(Keys.Model);
        private PropertyNode serverNode => container.Resolve<PropertyNode>(Keys.Server);
        private MainViewModel viewModel => container.Resolve<MainViewModel>();

        public PropertyView()
        {
            InitializeComponent();
            masterNode.Data = viewModel.Model;
            serverNode.Data = viewModel.Server;

            ScreensaverSend.Command = viewModel.SendScreensaver;
            LeaderboardSend.Command = viewModel.SendLeaderboard;
            PrizeWheelSend.Command = viewModel.SendPrizewheel;
            ServerConnect.Command = viewModel.Connect;

            viewModel.Subscribe(a =>
            {
                if (a is RefreshEvent)
                    refresh();
            });
        }



        // move to viewmodel
        private void refresh()
        {
            ServerGrid.Items.Clear();
            viewModel.TreeView(serverNode, ServerGrid).Subscribe(treeView =>
            {
            });

            {
                ScreensaverGrid.Items.Clear();
                PropertyHelper.FindNode(masterNode, a => a is PropertyBase { Name: nameof(Model.ScreenSaver) })
                      .Subscribe(node =>
                      {
                          viewModel.TreeView(node, ScreensaverGrid).Subscribe(treeView =>
                          {
                          });
                      });
            }

            {
                LeaderboardGrid.Items.Clear();
                PropertyHelper.FindNode(masterNode, a => a is PropertyBase { Name: nameof(Model.Leaderboard) })
                     .Subscribe(node =>
                     {
                         viewModel.TreeView(node, LeaderboardGrid).Subscribe(treeView =>
                         {
                         });
                     });
            }

            {
                PrizeWheelGrid.Items.Clear();
                PropertyHelper.FindNode(masterNode, a => a is PropertyBase { Name: nameof(Model.PrizeWheel) })

                    .Subscribe(node =>
                    {
                        viewModel.TreeView(node, PrizeWheelGrid).Subscribe(treeView =>
                        {
                        });
                    });
            }
        }
    }

    public record RefreshEvent();
}
