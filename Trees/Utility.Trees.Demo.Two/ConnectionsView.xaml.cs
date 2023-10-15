using Jellyfish;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utility.Common.Helper;
using Utility.Persists;
using Utility.Trees;
using Utility.WPF.Abstract;

namespace Utility.Trees.Demo.Two
{
    /// <summary>
    /// Interaction logic for ConnectionsUserControl.xaml
    /// </summary>
    public partial class ConnectionsView : UserControl
    {
        public static readonly DependencyProperty TreeViewProperty =
            DependencyProperty.Register("TreeView", typeof(TreeView), typeof(ConnectionsView), new PropertyMetadata(null, Changed));

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue is TreeView treeView && d is ConnectionsView connectionsView)
            {
                //    treeView < TreeView x: Name = "MyTreeView"   ItemsSource = "{Binding ViewModel.Items}"
                //      SelectedItemChanged = "MyTreeView_SelectedItemChanged"
                //          VirtualizingStackPanel.IsVirtualizing = "False"
                //          PreviewMouseUp = "MyTreeView1_PreviewMouseUp" >
                //</ TreeView >
                treeView.SelectedItemChanged += connectionsView.TreeView_SelectedItemChanged;
                treeView.PreviewMouseUp += connectionsView.TreeView_PreviewMouseUp;
                treeView.ItemsSource = connectionsView.viewModel.ViewModel.Items;
                connectionsView.TreeContent.Content = treeView;
            }
        }




        private Point? point0, point1;
        private LineViewModel last;

        LiteDbRepository repository = new(new(typeof(ConnectionModel), nameof(ConnectionModel.Id)));
        Converter converter;
        Dictionary<LineViewModel, LineViewModelInfo> dictionary = new();
        private readonly ConnectionsViewModel viewModel;


        public ConnectionsView()
        {
            InitializeComponent();

            //this.ItemsControl.ItemsSource = Lines;
            MouseMove += ConnectionsUserControl_MouseMove;
            var tree = new Tree(new ViewModel() { Name = "root" },
                    new Tree(new ViewModel { Name = "A" },
                    new Tree(new ViewModel { Name = "B" }),
                    new Tree(new ViewModel { Name = "C" }),
                    new Tree(new ViewModel { Name = "D" })));

            viewModel = new ConnectionsViewModel()
            {
                ServiceModel = new() { new Service { Name = "Service A" }, new Service { Name = "Service B" } },
                ViewModel = tree
            };
            DataContext = viewModel;
            converter = new Converter(this);

            Loaded += ConnectionsUserControl_Loaded;
        }

        private void ConnectionsUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in repository.All<ConnectionModel>())
            {
                viewModel.Connections.Add(item);
                var line = converter.To(item);
                viewModel.Lines.Add(line);
                dictionary.Add(line, new() { IsPersisted = true });
            }
        }



        public TreeView TreeView
        {
            get { return (TreeView)GetValue(TreeViewProperty); }
            set { SetValue(TreeViewProperty, value); }
        }




        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null)
                return;

            if (e.NewValue is not TreeViewItem { RenderSize: Size size } element)
            {
                element = TreeHelper.FindRecursive<TreeViewItem>(TreeView, e.NewValue) as TreeViewItem;
                size = element.RenderSize;
            }

            point0 = new Point(element.RenderSize.Width, TreeView.FindDepth(element));

            if (point1.HasValue)
            {
                point0 = point1 = null;
                last = null;

            }
            else
                Add();
        }

        private void TreeView2_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;
            if (e.AddedItems.Cast<object>().Single() is not FrameworkElement { RenderSize: var size } element)
            {
                element = TreeView2.ItemContainerGenerator.ContainerFromItem(e.AddedItems.Cast<object>().Single()) as FrameworkElement;
                size = element.RenderSize;
            }


            Point ofs = new(0, size.Height / 2d);
            var pointA = TreeView2.TransformToAncestor(this).Transform(ofs);
            var pointB = element.TransformToAncestor(TreeView2).Transform(ofs);
            point1 = new Point(pointA.X /*+ pointB.X*/, pointB.Y);
            if (point0.HasValue)
            {
                point0 = point1 = null;
                last = null;
            }
            else
                Add();
        }




        private void ConnectionsUserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (last != null)
            {
                // Perform the hit test against a given portion of the visual object tree.
                HitTestResult result = VisualTreeHelper.HitTest(this, e.GetPosition(this));
                var position = e.GetPosition(this); ;

                if (point0 == null)
                {
                    last.StartPoint = new Point(Math.Max(position.X, TreeView.ActualWidth), position.Y);
                }
                if (point1 == null)
                    last.EndPoint = new Point(Math.Min(position.X, Grid.ActualWidth - TreeView2.ActualWidth), position.Y);
            }
        }

        private void TreeView2_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Grid_MouseUp(sender, e);
        }

        private void TreeView_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Grid_MouseUp(sender, e);
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            for (var i = viewModel.Lines.Count - 1; i >= 0; i--)
            {
                var line = viewModel.Lines[i];
                if (dictionary.TryGetValue(line, out var persisted))
                {
                    if (persisted.IsPersisted == false)
                    {
                        if (converter.To(line) is ConnectionModel model)
                        {
                            repository.Add(model);
                            viewModel.Connections.Add(model);
                            persisted.IsPersisted = true;
                        }
                        else
                        {
                            dictionary.Remove(line);
                        }
                    }
                }
                else
                {
                    if (converter.To(line) is ConnectionModel model)
                    {
                        viewModel.Connections.Add(model);
                        repository.Add(model);
                        dictionary.Add(line, new() { IsPersisted = true });
                    }
                    else
                    {
                        viewModel.Lines.RemoveAt(i);
                    }
                }
            }
            TreeView2.SelectedItem = null;
            TreeView.ClearSelections();
        }

        void Add()
        {
            var pos = Mouse.GetPosition(this);
            viewModel.Lines.Add(last = new LineViewModel { Id = Guid.NewGuid(), StartPoint = point0.HasValue ? point0.Value : pos, EndPoint = point1.HasValue ? point1.Value : pos });
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Lines.Clear();
            viewModel.Connections.Clear();
            repository.Clear();
        }


        public class Converter
        {
            private readonly ConnectionsView userControl;
            private readonly ConnectionsViewModel dataContext;
            private Control hitResultsList;
            private Control hitResultsList2;

            public Converter(ConnectionsView userControl)
            {
                this.userControl = userControl;
                dataContext = userControl.DataContext as ConnectionsViewModel;
            }

            public LineViewModel To(ConnectionModel connectionViewModel)
            {

                Point pointA, pointB;
                Tree viewmodel = TreeHelper2.Match(dataContext.ViewModel, new((a) => (a.Data as IName).Name == connectionViewModel.ViewModelName)) as Tree;

                var treeViewItem = TreeHelper.FindRecursive<TreeViewItem>(userControl.TreeView, viewmodel);
                {
                    var y = userControl.TreeView.FindDepth(treeViewItem);
                    pointA = new Point(userControl.TreeView.ActualWidth, y);
                }
                IName service = null;
                foreach (var x in dataContext.ServiceModel)
                {
                    var data = x as IName;
                    if (connectionViewModel.ServiceName == (x as IName).Name)
                        service = data;
                }

                var listBoxItem = TreeHelper.FindRecursive<ListBoxItem>(userControl.TreeView2, service);
                {

                    Size size = listBoxItem.RenderSize;
                    Point ofs = new(0, size.Height / 2d);
                    pointB = listBoxItem.TransformToAncestor(userControl).Transform(ofs);

                }

                return new LineViewModel { Id = connectionViewModel.Id, StartPoint = pointA, EndPoint = pointB };
            }

            public ConnectionModel To(LineViewModel lineViewModel)
            {
                //HitTestResult result = VisualTreeHelper.HitTest(userControl, lineViewModel.StartPoint);
                VisualTreeHelper.HitTest(userControl, new HitTestFilterCallback(MyHitTestFilter), new HitTestResultCallback(MyHitTestResult), new PointHitTestParameters(lineViewModel.StartPoint));
                if (hitResultsList == null)
                {
                    MessageBox.Show($"{nameof(hitResultsList)} equals null");
                    return null;
                }
                var treeViewModel = (hitResultsList.DataContext as Tree).Data as ViewModel;
                //var end = VisualTreeHelper.HitTest(userControl, lineViewModel.EndPoint);
                VisualTreeHelper.HitTest(userControl.TreeView2, new HitTestFilterCallback(MyHitTestFilter2), new HitTestResultCallback(MyHitTestResult2), new PointHitTestParameters(new Point(10, lineViewModel.EndPoint.Y)));
                var service = hitResultsList2.DataContext as Service;
                return new ConnectionModel { Id = lineViewModel.Id, ServiceName = service.Name, ViewModelName = treeViewModel.Name };
            }

            // Filter the hit test values for each object in the enumeration.
            public HitTestFilterBehavior MyHitTestFilter(DependencyObject o)
            {
                // Test for the object value you want to filter.
                if (o is TreeViewItem treeViewItem)
                {
                    hitResultsList = treeViewItem;
                    return HitTestFilterBehavior.Continue;
                }
                else
                {
                    // Visual object is part of hit test results enumeration.
                    return HitTestFilterBehavior.Continue;
                }
            }

            // Return the result of the hit test to the callback.
            public HitTestResultBehavior MyHitTestResult(HitTestResult result)
            {
                // Add the hit test result to the list that will be processed after the enumeration.
                if (result.VisualHit is TreeViewItem treeViewItem)
                {
                    hitResultsList = treeViewItem;
                    return HitTestResultBehavior.Continue;
                }
                // Set the behavior to return visuals at all z-order levels.
                return HitTestResultBehavior.Continue;
            }

            public HitTestFilterBehavior MyHitTestFilter2(DependencyObject o)
            {
                // Test for the object value you want to filter.
                if (o is ListBoxItem treeViewItem)
                {
                    hitResultsList2 = treeViewItem;
                    return HitTestFilterBehavior.Stop;
                }
                else
                {
                    // Visual object is part of hit test results enumeration.
                    return HitTestFilterBehavior.Continue;
                }
            }

            // Return the result of the hit test to the callback.
            public HitTestResultBehavior MyHitTestResult2(HitTestResult result)
            {
                // Add the hit test result to the list that will be processed after the enumeration.
                if (result.VisualHit is ListBoxItem treeViewItem)
                {
                    hitResultsList2 = treeViewItem;
                    return HitTestResultBehavior.Stop;
                }
                // Set the behavior to return visuals at all z-order levels.
                return HitTestResultBehavior.Continue;
            }
        }
    }

    public static class TreeHelper
    {
        public static T? FindRecursive<T>(ItemsControl treeView, object instance) where T : Control
        {
            foreach (var item in treeView.Items)
            {
                if (item is T tItem)
                {
                    if (tItem.DataContext == instance)
                    {
                        return tItem;
                    }
                    else if (tItem is ItemsControl itemsControl)
                    {
                        if (FindRecursive<T>(itemsControl, instance) is T xx)
                            return xx;
                    }
                }
                else if (item == instance)
                {
                    if(treeView is TreeViewItem { IsExpanded: false} treeViewItem)
                    {
                        treeViewItem.IsExpanded = true;
                        treeViewItem.UpdateLayout();
                    }
                    var container = treeView.ItemContainerGenerator.ContainerFromItem(item);
                    return container as T;
                }
                else if (treeView.ItemContainerGenerator.ContainerFromItem(item) is ItemsControl itemsControl)
                {
                    var find = FindRecursive<T>(itemsControl, instance);
                    if (find is T xx)
                        return xx;
                }
            }
            return null;
        }
    }


    public class ConnectionsViewModel
    {
        public ObservableCollection<LineViewModel> Lines { get; } = new();
        public ObservableCollection<ConnectionModel> Connections { get; } = new();
        public Tree ViewModel { get; set; }

        public List<Service> ServiceModel { get; set; }
    }

    public class LineViewModelInfo
    {
        public bool IsPersisted { get; set; }
    }

    public class LineViewModel : Jellyfish.ViewModel
    {
        private Point _startPoint;

        public Guid Id { get; set; }

        public Point StartPoint
        {
            get { return _startPoint; }
            set
            {
                _startPoint = value;
                this.OnPropertyChanged();
            }
        }

        private Point _endPoint;
        public Point EndPoint
        {
            get { return _endPoint; }
            set
            {
                _endPoint = value;
                this.OnPropertyChanged();
            }
        }
    }


    public class ConnectionModel
    {
        public Guid Id { get; set; }

        public string ViewModelName { get; set; }

        public string ServiceName { get; set; }
    }



    public static class ClearTreeSelection
    {
        public static void ClearSelections(this TreeView tview)
        {

            ClearTreeViewItemsControlSelection(tview.Items, tview.ItemContainerGenerator);

            static void ClearTreeViewItemsControlSelection(ItemCollection ic, ItemContainerGenerator icg)
            {
                for (int i = 0; i < ic.Count; i++)
                {
                    // Get the TreeViewItem
                    if (icg.ContainerFromIndex(i) is TreeViewItem tvi)
                    {
                        //Recursive call to traverse deeper levels
                        ClearTreeViewItemsControlSelection(tvi.Items, tvi.ItemContainerGenerator);
                        //Deselect the TreeViewItem 
                        tvi.IsSelected = false;
                    }
                }
            }
        }

        public static double FindDepth(this TreeView treeView, TreeViewItem treeViewItem)
        {
            var headerControl = GetHeaderControl(treeViewItem);
            Point pointA;

            if (headerControl == null)
            {
                pointA = new(0, treeViewItem.RenderSize.Height / 2d);
            }
            else
            {
                var x = headerControl.RenderSize;
                Point ofs = new(0, x.Height / 2d);
                pointA = headerControl.TransformToAncestor(treeViewItem).Transform(ofs);

            }

            return treeViewItem.TransformToAncestor(treeView).Transform(pointA).Y;
        }

        public static FrameworkElement? GetHeaderControl(TreeViewItem item)
        {
            return (FrameworkElement?)item?.Template.FindName("PART_Header", item);
        }
    }
}