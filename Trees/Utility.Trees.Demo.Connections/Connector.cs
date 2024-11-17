using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utility.Common.Helper;
using Utility.Extensions;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.NonGeneric;
using Utility.Persists;
using Utility.Trees.Abstractions;
using Utility.WPF.Helpers;
using Utility.WPF.Reactives;

namespace Utility.Trees.Demo.Connections
{
    public partial class Connector
    {
        private Point? point0, point1;
        private LineViewModel? last;
        private LiteDbRepository repository = new(new(typeof(ConnectionModel), nameof(ConnectionModel.Id)));
        private Dictionary<LineViewModel, LineViewModelInfo> dictionary = [];
        private TreeViewItem hitResult;
        private TextBox hitResultT;
        private ItemsControl treeView;
        private Control hitResult2;
        private ItemsControl treeView2;
        private FrameworkElement container;

        public Connector()
        {
        }

        private void TreeView2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition((UIElement)sender);


            VisualTreeHelper.HitTest(this.TreeView2, new HitTestFilterCallback(MyHitTestFilter2), new HitTestResultCallback(MyHitTestResult2), new PointHitTestParameters(pt));
            if (hitResult2 == null)
                return;


            if (hitResult2 is not FrameworkElement { RenderSize: var size } element)
            {
                element = TreeView2.ItemContainerGenerator.ContainerFromItem(hitResult2) as FrameworkElement;
                size = element.RenderSize;
            }


            Point ofs = new(0, size.Height / 2d);
            var pointA = TreeView2.TransformToAncestor(Container).Transform(ofs);
            var pointB = element.TransformToAncestor(TreeView2).Transform(ofs);
            point1 = new Point(pointA.X /*+ pointB.X*/, pointB.Y);
            if (point0.HasValue)
            {
                point0 = point1 = null;
                last = null;
            }
            else
                Add(Direction.EndToStart);
        }

        private void TreeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition((UIElement)sender);
            VisualTreeHelper.HitTest(TreeView, new HitTestFilterCallback(MyHitTestFilter), new HitTestResultCallback(MyHitTestResult), new PointHitTestParameters(pt));
            if (hitResult == null)
            {
                MessageBox.Show($"{nameof(hitResult)} equals null");
                return;
            }

            if (hitResult is not TreeViewItem { RenderSize: Size size } element)
            {
                element = TreeView.FindRecursive<TreeViewItem>(hitResult) as TreeViewItem;
                size = element.RenderSize;
            }

            point0 = new Point(element.RenderSize.Width, TreeView.FindDistanceFromTop(element));

            if (hitResultT is not null)
            {
                hitResultT = null;
                return;
            }
            if (point1.HasValue)
            {
                point0 = point1 = null;
                last = null;

            }
            else
                Add(Direction.StartToEnd);
        }

        private void ConnectionsView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            last = null;
            if (viewModel.Lines.Any())
            {
                viewModel.Lines.RemoveAt(viewModel.Lines.Count - 1);
            }
        }

        public void Loaded()
        {
            viewModel.Connections.CollectionChanged += Connections_CollectionChanged;
            foreach (var item in repository.All<ConnectionModel>())
            {
                viewModel.Connections.Add(item);

            }

            void Connections_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                foreach (ConnectionModel item in e.NewItems)
                    To(item).Subscribe(line =>
                    {
                        viewModel.Lines.Add(line);
                        dictionary.Add(line, new LineViewModelInfo() { IsPersisted = true });
                    });
            }
        }

        public ItemsControl TreeView
        {
            get => treeView;
            set
            {
                treeView = value;
                treeView.PreviewMouseUp += TreeView_PreviewMouseUp;
                treeView.MouseRightButtonUp += TreeView_MouseRightButtonUp;
                treeView.PreviewMouseLeftButtonDown += TreeView_MouseDown;
            }
        }

        public ItemsControl TreeView2
        {
            get => treeView2; set
            {
                treeView2 = value;
                TreeView2.MouseRightButtonUp += ConnectionsView_MouseRightButtonUp;
                TreeView2.PreviewMouseLeftButtonDown += TreeView2_MouseDown;
                TreeView2.PreviewMouseUp += TreeView2_PreviewMouseUp;
            }
        }

        public FrameworkElement Container
        {
            get => container; set
            {
                container = value;
                Container.MouseMove += ConnectionsUserControl_MouseMove;
                Container.MouseRightButtonUp += ConnectionsView_MouseRightButtonUp;

                Container.SizeChanged += Container_SizeChanged;
            }
        }

        private void Container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var connection in viewModel.Connections)
            {
                To(connection)
                    .Subscribe(line =>
                    {
                        var x = viewModel.Lines.Single(a => a.Id == line.Id);
                        x.StartPoint = line.StartPoint;
                        x.EndPoint = line.EndPoint;
                    });
            }
        }

        public ConnectionsViewModel viewModel
        {
            get; set;
        }

        private void TreeView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            last = null;
        }

        private void ConnectionsUserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (last != null)
            {
                var position = e.GetPosition(Container); ;

                if (point0 == null)
                {
                    last.StartPoint = new Point(Math.Max(position.X, TreeView.ActualWidth - 10), position.Y);
                }
                if (point1 == null)
                    last.EndPoint = new Point(Math.Min(position.X, Container.ActualWidth - TreeView2.ActualWidth), position.Y);
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
                        if (To(line) is ConnectionModel model)
                        {
                            if (viewModel.Connections.Any(a => a.ServiceName == model.ServiceName && a.ViewModelName == model.ViewModelName && a.Movement == model.Movement))
                                return;

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
                    if (To(line) is ConnectionModel model)
                    {
                        if (viewModel.Connections.Any(a => a.ServiceName == model.ServiceName && a.ViewModelName == model.ViewModelName && a.Movement == model.Movement))
                            return;
                        viewModel.Connections.Add(model);
                        last = null;
                        repository.Add(model);
                        dictionary.Add(line, new() { IsPersisted = true });
                    }
                    else
                    {
                        viewModel.Lines.RemoveAt(i);
                    }
                }
            }
            TreeView.ClearSelections();
        }

        void Add(Direction direction)
        {
            var pos = Mouse.GetPosition(Container);
            viewModel.Lines.Add(last =
                new LineViewModel
                {
                    Id = Guid.NewGuid(),
                    StartPoint = point0.HasValue ? point0.Value : pos,
                    EndPoint = point1.HasValue ? point1.Value : pos,
                    Direction = direction
                });
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Lines.Clear();
            viewModel.Connections.Clear();
            repository.Clear();
        }

        public IObservable<LineViewModel> To(ConnectionModel connectionViewModel)
        {

            Point pointA, pointB;

            return Observable.Create<LineViewModel>(observer =>
            {
                if (viewModel.Tree is ObservableTree oTree)
                {
                    return oTree.MatchDescendant(new((a) => (a.Data as IName).Name == connectionViewModel.ViewModelName))
                        .Subscribe(a =>
                        {
                            MatchTree(a).Subscribe(observer);
                        });
                }
                else
                {
                    var d = viewModel.Tree.MatchDescendant(new((a) => (a.Data as IName).Name == connectionViewModel.ViewModelName));
                    MatchTree(d).Subscribe(observer);
                    return Disposable.Empty;
                }
            });

            IObservable<LineViewModel> MatchTree(IReadOnlyTree viewmodel)
            {
                return Observable.Create<LineViewModel>(observer =>
                {
                    var treeViewItem = TreeView.FindRecursive<TreeViewItem>(viewmodel);

                    if (treeViewItem == null)
                    {
                        TreeView.FindRecursiveAsync<TreeViewItem>(viewmodel).Subscribe(treeViewItem =>
                        {
                            observer.OnNext(match(treeViewItem));
                            observer.OnCompleted();
                        });
                    }
                    else
                    {
                        observer.OnNext(match(treeViewItem));
                    }
                    return Disposable.Empty;
                });



                LineViewModel match(TreeViewItem treeViewItem)
                {
                    var y = TreeView.FindDistanceFromTop(treeViewItem);
                    pointA = new Point(TreeView.ActualWidth, y);

                    IName service = null;
                    foreach (var x in viewModel.ServiceModel)
                    {
                        var data = x as IName;
                        if (connectionViewModel.ServiceName == (x as IName).Name)
                            service = data;
                    }

                    var listBoxItem = TreeView2.FindRecursive<ListBoxItem>(service);

                    Size size = listBoxItem.RenderSize;
                    Point ofs = new(0, size.Height / 2d);
                    pointB = listBoxItem.TransformToAncestor(Container).Transform(ofs);

                    return new LineViewModel { Id = connectionViewModel.Id, StartPoint = pointA, EndPoint = pointB, Direction = connectionViewModel.Movement == Movement.FromViewModelToService ? Direction.StartToEnd : Direction.EndToStart };
                }
            }
        }

        public ConnectionModel? To(LineViewModel lineViewModel)
        {
            VisualTreeHelper.HitTest(TreeView, new HitTestFilterCallback(MyHitTestFilter), new HitTestResultCallback(MyHitTestResult), new PointHitTestParameters(lineViewModel.StartPoint));
            if (hitResult == null)
            {
                MessageBox.Show($"{nameof(hitResult)} equals null");
                return null;
            }

            if ((hitResult.DataContext as Tree).Data is IName { Name: { } name })
            {
                VisualTreeHelper.HitTest(TreeView2, new HitTestFilterCallback(MyHitTestFilter2), new HitTestResultCallback(MyHitTestResult2), new PointHitTestParameters(new Point(10, lineViewModel.EndPoint.Y)));
                if (hitResult2 == null)
                    return null;
                var service = hitResult2.DataContext as Service;
                return new ConnectionModel { Id = lineViewModel.Id, ServiceName = service.Name, ViewModelName = name, Movement = lineViewModel.Direction == Direction.StartToEnd ? Movement.FromViewModelToService : Movement.ToViewModelFromService };
            }
            throw new Exception("s66d es3333!FV$");
        }

        public HitTestFilterBehavior MyHitTestFilter(DependencyObject o)
        {
            // Test for the object value you want to filter.
            if (o is TreeViewItem { Items: var items } treeViewItem)
            {
                hitResult = treeViewItem;
                if (items.Any() == false)
                    return HitTestFilterBehavior.Stop;
                else
                    return HitTestFilterBehavior.Continue;
            }
            else
            {
                // Visual object is part of hit test results enumeration.
                return HitTestFilterBehavior.Continue;
            }
        }

        public HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            // Add the hit test result to the list that will be processed after the enumeration.
            if (result.VisualHit is TreeViewItem treeViewItem)
            {
                hitResult = treeViewItem;
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
                hitResult2 = treeViewItem;
                return HitTestFilterBehavior.Stop;
            }
            else
            {
                // Visual object is part of hit test results enumeration.
                return HitTestFilterBehavior.Continue;
            }
        }

        public HitTestResultBehavior MyHitTestResult2(HitTestResult result)
        {
            // Add the hit test result to the list that will be processed after the enumeration.
            if (result.VisualHit is ListBoxItem treeViewItem)
            {
                hitResult2 = treeViewItem;
                return HitTestResultBehavior.Stop;
            }
            // Set the behavior to return visuals at all z-order levels.
            return HitTestResultBehavior.Continue;
        }
    }

    public class ConnectionsViewModel
    {
        public ObservableCollection<LineViewModel> Lines { get; } = new();
        public ObservableCollection<ConnectionModel> Connections { get; } = new();
        public Tree Tree { get; set; }

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

        public Direction Direction { get; internal set; }
    }


    public class ConnectionModel
    {
        public Guid Id { get; set; }

        public string ViewModelName { get; set; }

        public string ServiceName { get; set; }
        public Movement Movement { get; internal set; }
    }



    public static class ClearTreeSelection
    {
        public static void ClearSelections(this ItemsControl tview)
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

        public static double FindDistanceFromTop(this ItemsControl treeView, Control treeViewItem)
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

        public static FrameworkElement? GetHeaderControl(Control item)
        {
            return (FrameworkElement?)item?.Template.FindName("PART_Header", item);
        }
    }

    public enum Direction
    {
        StartToEnd, EndToStart
    }
    public enum Movement
    {
        FromViewModelToService, ToViewModelFromService
    }
}


