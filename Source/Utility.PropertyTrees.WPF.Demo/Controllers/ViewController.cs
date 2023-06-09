using DryIoc;
using Swordfish.NET.Collections.Auxiliary;

using Utility.Infrastructure;
using Utility.Nodes;
using Utility.PropertyTrees.WPF.Demo.Views;
using Utility.PropertyTrees.WPF.Meta;
using Utility.Observables.Generic;

namespace Utility.PropertyTrees.WPF.Demo
{
    public class ViewController : BaseObject
    {
        private TreeView treeView;
        private RootProperty node;

        public override Key Key => new Key(Guids.ViewController, nameof(ViewController), typeof(ViewController));


        public void OnNext(StartEvent startEvent)
        {
            var grid = CreateContent(node = startEvent.Property);
            var window = new Window { Content = grid };
            //ModernWpf.Controls.Primitives.WindowHelper.SetUseModernWindowStyle(window, true);
            window.Show();

            object CreateContent(ValueNode valueNode)
            {
                treeView = new TreeView();

                Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(treeView, valueNode))
                   .Subscribe(a =>
                   {
                       //modelViewModel.IsConnected = true;
                       //System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                   });

                var grid = new Grid();
                grid.RowDefinitions.AddRange(new[] {
                new RowDefinition { Height = new GridLength(30, GridUnitType.Auto) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) } });

                var subGrid = new Grid() { };
                var commandView = new CommandView();
                Grid.SetRow(subGrid, 1);
                grid.Children.Add(commandView);
                grid.Children.Add(subGrid);
                subGrid.Children.Add(treeView);

                return grid;
            }
        }

        public void OnNext(RefreshRequest request)
        {
            treeView.Items.Clear();
            //Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(treeView, node))
            //    .Subscribe(a =>
            //    {
            //        //modelViewModel.IsConnected = true;
            //        //System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            //    });
        }
    }
}
