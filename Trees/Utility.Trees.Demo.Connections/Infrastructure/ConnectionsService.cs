using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utility.Common.Helper;
using Utility.Helpers.NonGeneric;
using Utility.Helpers.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Persists;
using Utility.PropertyNotifications;
using Utility.Trees.Abstractions;
using Utility.WPF.Helpers;
using Utility.WPF.Reactives;
using Utility.Reactives;
using DynamicData;
using System.Collections.Specialized;
using Utility.Trees.Extensions.Async;

namespace Utility.Trees.Demo.Connections
{
    public class ConnectionsService
    {


        private LiteDbRepository repository = new(new(typeof(ConnectionModel), nameof(ConnectionModel.Id)));
        private Dictionary<LineViewModel, LineViewModelInfo> dictionary = [];

        private TreeViewItem hitResult;
        private Control hitResult2;

        private ItemsControl treeView;

        private NodeGroup treeView2;
        private FrameworkElement container;

        public ConnectionsService()
        {
        }

        public ConnectionsViewModel viewModel { get; set; }

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

        public NodeGroup TreeView2
        {
            get => treeView2; set
            {
                treeView2 = value;

                treeView2.ConnectorChange += TreeView2_ConnectorChange;
                treeView2.NodeChange += TreeView2_NodeChange;
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
                Container.MouseUp += Container_PreviewMouseUp;
            }
        }



        void Log([CallerMemberName] string? caller = null)
        {
            var now = DateTime.Now;
            while (viewModel.Logs.Count > 0 && viewModel.Logs.ElementAt(0).DateTime.Second != now.Second)
                viewModel.Logs.RemoveAt(0);
            viewModel.Logs.Add(new Log(caller, now));
        }

        private void TreeView2_ConnectorChange(object source, ConnectorChangeRoutedEventArgs rangeChange)
        {
            Log();


            hitResult2 = rangeChange.Connector;
            var size = hitResult2.RenderSize;
            Point ofs = new(0, hitResult2.ActualHeight / 2d);
            var pointB = TreeView2.TransformToAncestor(Container).Transform(ofs);
            viewModel.Point1 = hitResult2.TransformToAncestor(TreeView2).Transform(pointB);


            if (viewModel.Point0.HasValue)
            {
                viewModel.Point0 = viewModel.Point1 = null;
                Grid_MouseUp(default, default);
            }
            else
                Add(Direction.EndToStart);
            
            //dsf2(rangeChange);
            //dsf(rangeChange);

        }

        void dsf2()
        {
            foreach (var connectionModel in viewModel.Lines)
            {
                connectionModel.IsSelected = false;// ((rangeChange.Connector.DataContext is Parameter { Name: { } name, ServiceName: { } serviceName }) && connectionModel.ParameterName == name && connectionModel.ServiceName == serviceName);
            }

        }


        void dsf()
        {
            foreach (var connectionModel in viewModel.Connections)
            {
                connectionModel.IsSelected = false;//((rangeChange.Connector.DataContext is Parameter { Name: { } name, ServiceName: { } serviceName }) && connectionModel.ParameterName == name && connectionModel.ServiceName == serviceName);
            }

        }


        private void TreeView2_NodeChange(object source, NodeChangeRoutedEventArgs rangeChange)
        {
            Log();
            foreach (var connectionModel in viewModel.Connections)
            {
                if (connectionModel.ServiceName == (rangeChange.Node.DataContext as Service).Name)
                {
                    connectionModel.IsSelected = true;
                    var line = viewModel.Lines.Single(a => a.Id == connectionModel.Id);
                    line.IsSelected = true;
                }
                else
                {
                    connectionModel.IsSelected = false;
                    var line = viewModel.Lines.Single(a => a.Id == connectionModel.Id);
                    line.IsSelected = false;
                }
            }
        }

        private void TreeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Log();
            Point pt = e.GetPosition((UIElement)sender);
            VisualTreeHelper.HitTest(TreeView, new HitTestFilterCallback(MyHitTestFilter), new HitTestResultCallback(MyHitTestResult), new PointHitTestParameters(pt));
            if (hitResult == null)
            {
                //MessageBox.Show($"{nameof(hitResult)} equals null");
                return;
            }

            if (hitResult is not TreeViewItem { RenderSize: Size size } element)
            {
                element = TreeView.FindRecursive<TreeViewItem>(hitResult) as TreeViewItem;
                size = element.RenderSize;
            }

            viewModel.Point0 = TreeView.FindLocation(element);


            if (viewModel.Point1.HasValue)
            {
                viewModel.Point0 = viewModel.Point1 = null;
                viewModel.Last = null;

            }
            else
                Add(Direction.StartToEnd);

            dsf2();
            dsf();
        }

        private void ConnectionsView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Log();
            if (viewModel.Last is null)
                return;
            viewModel.Last = null;
            if (viewModel.Lines.Any())
            {
                viewModel.Lines.RemoveAt(viewModel.Lines.Count - 1);
            }
        }

        public void Loaded()
        {
            Log();

            foreach (var item in repository.All<ConnectionModel>())
            {
                viewModel.Connections.Add(item);
                From(item).Subscribe(line =>
                {
                    viewModel.Lines.Add(line);
                    dictionary.Add(line, new LineViewModelInfo() { IsPersisted = true });
                    InitialiseLine(line);
                    Initialise(item);
                });
            }

            viewModel.Connections.Additions<ConnectionModel>().Subscribe(Initialise);

            viewModel.Lines.Additions<LineViewModel>().Subscribe(InitialiseLine);


            void Initialise(ConnectionModel item)
            {
                item.WithChangesTo(a => a.IsDeleted)
                    .Where(a => a)
                    .Subscribe(a =>
                    {
                        viewModel.Connections.Remove(item);
                        viewModel.Lines.RemoveBy(a => a.Id == item.Id);
                        repository.Remove(item);
                    });

                item.WithChangesTo(a => a.IsSelected)
                    .Subscribe(a => viewModel.Lines.Single(a => a.Id == item.Id).IsSelected = a);
            }



            void InitialiseLine(LineViewModel item)
            {
                item.WithChangesTo(a => a.IsSelected)
                    .Subscribe(a => viewModel.Connections
                        .AndChanges<ConnectionModel>()
                        .Subscribe(set =>
                        {
                            foreach (var s in set)
                            {
                                if(s.Value.Id == item.Id)
                                    s.Value.IsSelected = a;
                            }
                        }));
            }
        }




        private void Container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Log();
            foreach (var connection in viewModel.Connections)
            {
                From(connection)
                    .Subscribe(line =>
                    {
                        var x = viewModel.Lines.Single(a => a.Id == line.Id);
                        x.StartPoint = line.StartPoint;
                        x.EndPoint = line.EndPoint;
                    });
            }
        }


        private void TreeView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ConnectionsView_MouseRightButtonUp(default, default);
        }

        private void ConnectionsUserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (viewModel.Last != null)
            {
                var position = e.GetPosition(Container); ;

                if (viewModel.Point0 == null)
                {
                    viewModel.Last.StartPoint = new Point(position.X, position.Y);
                }
                if (viewModel.Point1 == null)
                    viewModel.Last.EndPoint = new Point(position.X, position.Y);
            }
        }

        private void Container_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Log();
            Grid_MouseUp(sender, e);
        }

        private void TreeView2_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Log();
            //Grid_MouseUp(sender, e);
        }

        private void TreeView_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Log();
            //Grid_MouseUp(sender, e);
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Log();
            if (viewModel.Last == null)
            {
                return;
            }
            if (To(viewModel.Last) is ConnectionModel model)
            {

                if (viewModel.Connections.Any(a => a.ServiceName == model.ServiceName && a.ViewModelName == model.ViewModelName && a.Movement == model.Movement && a.ParameterName == model.ParameterName))
                {
                    viewModel.Lines.Remove(viewModel.Last);
                }
                else
                {
                    viewModel.Connections.Add(model);
                    repository.Add(model);
                    dictionary.Add(viewModel.Last, new() { IsPersisted = true });
                }
            }
            else
            {
                viewModel.Lines.Remove(viewModel.Last);
            }


            if (viewModel.Lines.Count > viewModel.Connections.Count)
            {

            }

            viewModel.Last = null;
            viewModel.Point0 = null;
            viewModel.Point1 = null;
            TreeView.ClearSelections();
        }

        void Add(Direction direction)
        {
            Log();
            var pos = Mouse.GetPosition(Container);
            viewModel.Lines.Add(viewModel.Last =
                new LineViewModel
                {
                    Id = Guid.NewGuid(),
                    StartPoint = viewModel.Point0.HasValue ? viewModel.Point0.Value : pos,
                    EndPoint = viewModel.Point1.HasValue ? viewModel.Point1.Value : pos,
                    Direction = direction,
                    IsSelected = true
                });
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Log();
            viewModel.Lines.Clear();
            viewModel.Connections.Clear();
            repository.Clear();
        }

        public IObservable<LineViewModel> From(ConnectionModel connectionViewModel)
        {
            Log();
            Point pointA, pointB;

            return Observable.Create<LineViewModel>(observer =>
            {
                //if (viewModel.Tree is INotifyCollectionChanged oTree)
                //{
                    return viewModel.Tree.Descendant(new((a) => (a.tree.Data as IName).Name == connectionViewModel.ViewModelName))
                        .Subscribe(a =>
                        {
                            MatchTree(a.NewItem).Subscribe(observer);
                        });
                //}
                //else
                //{
                //    var d = viewModel.Tree.MatchDescendant(new((a) => (a.Data as IName).Name == connectionViewModel.ViewModelName));
                //    MatchTree(d).Subscribe(observer);
                //    return Disposable.Empty;
                //}
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
                    var pointA = TreeView.FindLocation(treeViewItem);

                    IName service = null;
                    foreach (var x in viewModel.ServiceModel)
                    {
                        var data = x as IName;
                        if (connectionViewModel.ServiceName == (x as IName).Name)
                        {
                            service = data;
                            break;
                        }
                    }

                    var listBoxItem = TreeView2.FindRecursive<Node>(service);

                    foreach (var input in listBoxItem.Inputs)
                    {
                        if (input is Parameter { Name: { } name } && name == connectionViewModel.ParameterName)
                        {
                            return convert(connectionViewModel, pointA, listBoxItem);
                        }
                    }

                    foreach (var input in listBoxItem.Outputs)
                    {
                        if (input is Parameter { Name: { } name } && name == connectionViewModel.ParameterName)
                        {
                            return convert(connectionViewModel, pointA, listBoxItem);

                        }
                    }

                    throw new Exception("DFG 34333");
                }
            }

            LineViewModel convert(ConnectionModel connectionViewModel, Point pointA, Node? listBoxItem)
            {
                var control = VisualTreeExHelper.FindChildren<Connector>(listBoxItem).Single(a => (a.DataContext as Parameter).Name == connectionViewModel.ParameterName);
                //Size size = control.RenderSize;
                Point ofs = new(0, control.ActualHeight / 2d);
                var point_B = TreeView2.TransformToAncestor(Container).Transform(ofs);
                var pointB = control.TransformToAncestor(TreeView2).Transform(point_B);
                pointB = new Point(pointB.X, pointB.Y);

                return new LineViewModel { Id = connectionViewModel.Id, StartPoint = pointA, EndPoint = pointB, Direction = connectionViewModel.Movement == Movement.FromViewModelToService ? Direction.StartToEnd : Direction.EndToStart };
            }
        }



        public ConnectionModel? To(LineViewModel lineViewModel)
        {
            Log();
            hitResult = null;
            VisualTreeHelper.HitTest(TreeView, new HitTestFilterCallback(MyHitTestFilter), new HitTestResultCallback(MyHitTestResult), new PointHitTestParameters(lineViewModel.StartPoint));
            if (hitResult == null)
            {
                //MessageBox.Show($"{nameof(hitResult)} equals null");
                return null;
            }

            if ((hitResult.DataContext as Tree).Data is IName { Name: { } name })
            {
                hitResult2 = null;
                VisualTreeHelper.HitTest(Container, new HitTestFilterCallback(MyHitTestFilter2), new HitTestResultCallback(MyHitTestResult2), new PointHitTestParameters(new Point(lineViewModel.EndPoint.X + 5, lineViewModel.EndPoint.Y)));
                if (hitResult2 == null)
                    return null;

                var parameter = hitResult2.DataContext as Parameter;
                return new ConnectionModel { Id = lineViewModel.Id, ServiceName = parameter.ServiceName, ParameterName = parameter.Name, ViewModelName = name, Movement = lineViewModel.Direction == Direction.StartToEnd ? Movement.FromViewModelToService : Movement.ToViewModelFromService };
            }
            throw new Exception("s66d es3333!FV$");

            HitTestFilterBehavior MyHitTestFilter2(DependencyObject o)
            {
                // Test for the object value you want to filter.
                if (o is Connector treeViewItem)
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

            HitTestResultBehavior MyHitTestResult2(HitTestResult result)
            {
                // Add the hit test result to the list that will be processed after the enumeration.
                if (result.VisualHit is Connector treeViewItem)
                {
                    hitResult2 = treeViewItem;
                    return HitTestResultBehavior.Stop;
                }
                // Set the behavior to return visuals at all z-order levels.
                return HitTestResultBehavior.Continue;
            }
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



    }
}


