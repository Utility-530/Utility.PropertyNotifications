using System.Reactive.Linq;
using DryIoc;
using Utility.PropertyTrees.Demo.Model;
using Utility.Observables.Generic;
using Utility.Observables.NonGeneric;
using Utility.Nodes;
using System.Reactive.Disposables;
using System.Net;
using Utility.PropertyTrees.Services;

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

        private ValueNode masterNode => container.Resolve<RootModelProperty>();
     
        private IModelController controller => container.Resolve<IModelController>();
        //ModelViewModel modelViewModel => container.Resolve<ModelViewModel>();

        public PropertyView(IContainer  container)
        {
 
            InitializeComponent();
          
            //this.DataContext = modelViewModel;
          
            controller.Subscribe(a =>
            {
                if (a is RefreshEvent)
                    refresh();
            });

            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
 

        }





        IDisposable? disposable;
        // move to viewmodel
        private void refresh()
        {
            disposable?.Dispose();
            CompositeDisposable disposables = new();
            {
                ServerGrid.Items.Clear();
                controller.TreeView(masterNode, ServerGrid).Subscribe(treeView =>
                {

                    //modelViewModel.IsConnected = true;
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                });
            }

            //{
            //    ScreensaverGrid.Items.Clear();
            //    PropertyExplorer.FindNode(masterNode, a => a is PropertyBase { Name: nameof(GameModel.ScreenSaver) }, out var dis)
            //          .Subscribe(node =>
            //          {

            //              controller.TreeView(node, ScreensaverGrid).Subscribe(treeView =>
            //              {
            //              });
            //          });
            //}

            //{
            //    LeaderboardGrid.Items.Clear();
            //    PropertyExplorer.FindNode(masterNode, a => a is PropertyBase { Name: nameof(GameModel.Leaderboard) }, out var dis)
            //         .Subscribe(node =>
            //         {
            //             controller.TreeView(node, LeaderboardGrid)
            //             .Subscribe(treeView =>
            //             {
            //             }).DisposeWith(disposables);
            //         }).DisposeWith(disposables);
            //    disposables.Add(dis);
            //}

            //{
            //    PrizeWheelGrid.Items.Clear();
            //    PropertyExplorer.FindNode(masterNode, a => a is PropertyBase { Name: nameof(GameModel.PrizeWheel) }, out var dis)
            //        .Subscribe(node =>
            //        {

            //            controller.TreeView(node, PrizeWheelGrid).Subscribe(treeView =>
            //            {
            //            });
            //        });
            //}
            disposable = disposables;

        }
    }

}
