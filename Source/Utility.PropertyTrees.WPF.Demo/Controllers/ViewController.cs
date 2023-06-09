using Swordfish.NET.Collections.Auxiliary;

using Utility.Infrastructure;
using Utility.Nodes;
using Utility.PropertyTrees.WPF.Demo.Views;
using Utility.PropertyTrees.WPF.Meta;
using Utility.Observables.Generic;
using static Utility.PropertyTrees.Events;
using System.Windows.Input;
using System.Windows.Media;
using Utility.WPF.Adorners.Infrastructure;

namespace Utility.PropertyTrees.WPF.Demo
{
    public class ViewController : BaseObject
    {
        private TreeView treeView;
        private RootProperty node;
        private Grid grid;
        private DataGrid dataGrid = new();

        public override Key Key => new(Guids.ViewController, nameof(ViewController), typeof(ViewController));


        public void OnNext(StartEvent startEvent)
        {
            var content = CreateContent(node = startEvent.Property);
            var window = new Window { Content = content };
            //ModernWpf.Controls.Primitives.WindowHelper.SetUseModernWindowStyle(window, true);
            window.Show();

            object CreateContent(ValueNode valueNode)
            {
                treeView = new();
                treeView.MouseDoubleClick += new MouseButtonEventHandler(treView_MouseDoubleClick);
                Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(treeView, valueNode))
                   .Subscribe(a =>
                   {
                       //modelViewModel.IsConnected = true;
                       //System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                   });

                grid = new Grid();
                grid.RowDefinitions.AddRange(new[] {
                new RowDefinition { Height = new GridLength(30, GridUnitType.Auto) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) } ,
                new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) } });

                var subGrid = new Grid() { };
                var commandView = new CommandView();
                Grid.SetRow(subGrid, 1);
                Grid.SetRow(dataGrid, 2);
                grid.Children.Add(commandView);
                grid.Children.Add(subGrid);
                grid.Children.Add(dataGrid);
                subGrid.Children.Add(treeView);


                var adorner = new Button
                {
                    Content = "click",                  
                    CommandParameter = new TreeClickEvent(node),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top
                };

                AdornerHelper.AddIfMissingAdorner(dataGrid, adorner);
                dataGrid.SetValue(AdornerEx.IsEnabledProperty, true);
                adorner.Click += Adorner_Click;

                return grid;
            }
        }

        private void Adorner_Click(object sender, RoutedEventArgs e)
        {
            this.Observe<SetViewModelResponse, SetViewModelRequest>(new(valueNode.Key, Response.ViewModels.Single()))
             .Subscribe(response =>
             {    
             });
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

        public void OnNext(TreeMouseDoubleClickEvent command)
        {

        }

        public GetViewModelResponse Response;
        private ValueNode valueNode;

        /// <summary>
        /// <a href="https://social.msdn.microsoft.com/Forums/vstudio/en-US/c01df033-0acd-4690-a24d-3b7090694bc0/how-can-handle-treeviewitems-mouseclick-or-mousedoubleclick-event?forum=wpf"></a>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (GetSelectedItem((FrameworkElement)e.OriginalSource, treeView) is { Header: ValueNode valueNode } treeviewItem)
            {
                this.valueNode = valueNode;
                treeviewItem.IsExpanded = !treeviewItem.IsExpanded;
                this.Observe<GetViewModelResponse, GetViewModelRequest>(new(valueNode.Key))
                    .Subscribe(response =>
                    {
                        Response = response;
                        dataGrid.Visibility = Visibility.Visible;
                        dataGrid.ItemsSource = response.ViewModels;
                    });
            }

            TreeViewItem GetSelectedItem(UIElement sender, UIElement objTreeViewControl)
            {
                Point point = sender.TranslatePoint(new Point(0, 0), objTreeViewControl);
                var isHitTestAvailable = objTreeViewControl.InputHitTest(point) as DependencyObject;
                while (isHitTestAvailable != null && !(isHitTestAvailable is TreeViewItem))
                {
                    isHitTestAvailable = VisualTreeHelper.GetParent(isHitTestAvailable);
                }
                return isHitTestAvailable as TreeViewItem;
            }
        }

    }
}
