using System.Reactive.Linq;
using DryIoc;
using Utility.PropertyTrees.Demo.Model;
using Utility.PropertyTrees.Infrastructure;
using Utility.Observables.Generic;
using Utility.Observables.NonGeneric;
using Utility.Nodes;

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

        private ValueNode masterNode => container.Resolve<ModelProperty>();
        private ValueNode serverNode => container.Resolve<ServerProperty>();
        private IModelController controller => container.Resolve<IModelController>();
        ModelViewModel modelViewModel => container.Resolve<ModelViewModel>();

        public PropertyView(IContainer  container)
        {
 
            InitializeComponent();
          
            this.DataContext = modelViewModel;
          
            controller.Subscribe(a =>
            {
                if (a is RefreshEvent)
                    refresh();
            });

            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
 

        }






        // move to viewmodel
        private void refresh()
        {
            ServerGrid.Items.Clear();
            //controller.TreeView(serverNode, ServerGrid).Subscribe(treeView =>
            //{
            //    modelViewModel.IsConnected = true;
            //    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            //});

   
            //{
            //    ScreensaverGrid.Items.Clear();
            //    PropertyExplorer.FindNode(masterNode, a => a is PropertyBase { Name: nameof(Model.ScreenSaver) })
            //          .Subscribe(node =>
            //          {
            //              controller.TreeView(node, ScreensaverGrid).Subscribe(treeView =>
            //              {
            //              });
            //          });
            //}

            {
                LeaderboardGrid.Items.Clear();
                PropertyExplorer.FindNode(masterNode, a => a is PropertyBase { Name: nameof(Model.Leaderboard) })
                     .Subscribe(node =>
                     {
                         controller.TreeView(node, LeaderboardGrid).Subscribe(treeView =>
                         {
                         });
                     });
            }

            //{
            //    PrizeWheelGrid.Items.Clear();
            //    PropertyExplorer.FindNode(masterNode, a => a is PropertyBase { Name: nameof(Model.PrizeWheel) })

            //        .Subscribe(node =>
            //        {
            //            controller.TreeView(node, PrizeWheelGrid).Subscribe(treeView =>
            //            {
            //            });
            //        });
            //}
        }
    }

}
