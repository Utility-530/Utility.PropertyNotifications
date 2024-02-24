using Utility.Infrastructure;
using Utility.Nodes;
using Utility.PropertyTrees.WPF.Meta;
using Utility.Observables.Generic;
using Utility.Trees.Abstractions;
using Utility.WPF.Reactives;

namespace Utility.PropertyTrees.WPF.Demo
{
    public class ViewController : BaseObject
    {
        private TreeView treeView;
        private RootProperty node;
        private Grid grid;
        private TreeView dataGrid = new();
        private GetViewModelResponse response;
        private ValueNode valueNode;

        public override Key Key => new(Guids.ViewController, nameof(ViewController), typeof(ViewController));

        public void OnNext(StartEvent startEvent)
        {
            Context.Post((_) =>
            {
                var content = CreateContent(node = startEvent.Property);
                var window = new Window { Content = content };
                window.Show();
            }, default);
            //ModernWpf.Controls.Primitives.WindowHelper.SetUseModernWindowStyle(window, true);


            object CreateContent(ValueNode valueNode)
            {
                treeView = new();
                CreateSelected(valueNode);

                grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                Grid.SetRow(treeView, 0);
                grid.Children.Add(treeView);

#if DEBUG
                //CreateDebugContent();
#endif
                return grid;
            }
        }
        //    void CreateDebugContent()
        //    {
        //        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
        //        var refreshAdorner = new Button
        //        {
        //            Content = "refresh",
        //            CommandParameter = new TreeClickEvent(node),
        //            HorizontalAlignment = HorizontalAlignment.Right,
        //            VerticalAlignment = VerticalAlignment.Top
        //        };

        //        AdornerHelper.AddIfMissingAdorner(grid, refreshAdorner);
        //        grid.SetValue(AdornerEx.IsEnabledProperty, true);

        //        refreshAdorner.Click += (s, e) =>
        //        {
        //            Send(new RefreshRequest());            
        //        };

        //        Grid.SetRow(dataGrid, 1);
        //        grid.Children.Add(dataGrid);

        //        var adorner = new Button
        //        {
        //            Content = "update",
        //            CommandParameter = new TreeClickEvent(node),
        //            HorizontalAlignment = HorizontalAlignment.Right,
        //            VerticalAlignment = VerticalAlignment.Top
        //        };

        //        AdornerHelper.AddIfMissingAdorner(dataGrid, adorner);
        //        dataGrid.SetValue(AdornerEx.IsEnabledProperty, true);
        //        adorner.Click += Adorner_Click;
        //    }
        //}

        private void CreateSelected(ValueNode valueNode)
        {
            this.valueNode = valueNode;
            Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(treeView, valueNode))
                .Subscribe(a =>
                {
                });

            treeView
                .MouseDoubleClicks()
                .Subscribe(a =>
                {
                    //if (a is { Header: ValueNode valueNode } treeviewItem)
                    //{
                    //    this.valueNode = valueNode;
                    //    treeviewItem.IsExpanded = !treeviewItem.IsExpanded;
                    //    this.Observe<GetViewModelResponse, GetViewModelRequest>(new(valueNode.Key))
                    //        .Subscribe(response =>
                    //        {
                    //            Context.Post((_) =>
                    //            {
                    //                this.response = response;
                    //                dataGrid.Visibility = Visibility.Visible;
                    //                //dataGrid.ItemsSource = response.ViewModels;
                    //                var root = new RootProperty(Guid.NewGuid()) { Data = new { response.ViewModels } };
                    //                Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(dataGrid, root))
                    //                .Subscribe(a =>
                    //                {
                    //                    //modelViewModel.IsConnected = true;
                    //                    //System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                    //                });
                    //            }, default);
                    //            //dataGrid.ItemsSource = response.ViewModels;
                    //        });
                    //}
                });

            treeView
                .MouseSingleClicks()
                .Subscribe(a =>
                {
                    if (a is { Header: ITree { } node })
                    {
                        Send(new ClickChange(a, node));
                    }
                });

            treeView
                .MouseMoves()
                .Subscribe(a =>
                {
                    if (a.item is { Header: ITree { } node })
                    {
                        Structs.Point sPoint = new(a.point.X, a.point.Y);
                        Send(new OnHoverChange(a.item, node, true, sPoint));
                    }
                });

            treeView
                .MouseHoverLeaves()
                .Subscribe(a =>
                {
                    if (a.item is { Header: ITree { } node })
                    {
                        Send(new OnHoverChange(a, node, false, default));
                    }
                });
        }

        //private void Adorner_Click(object sender, RoutedEventArgs e)
        //{
        //    this.Observe<SetViewModelResponse, SetViewModelRequest>(new(valueNode.Key, response.ViewModels.Single()))
        //        .Subscribe(response =>
        //        {
        //        });
        //}

        public void OnNext(RefreshRequest request)
        {
            treeView.Items.Clear();
            Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(treeView, valueNode))
            .Subscribe();
            //Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(treeView, node))
            //    .Subscribe(a =>
            //    {
            //        //modelViewModel.IsConnected = true;
            //        //System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            //    });
        }

        public void OnNext(TreeMouseDoubleClickEvent command)
        {

        }
    }
}