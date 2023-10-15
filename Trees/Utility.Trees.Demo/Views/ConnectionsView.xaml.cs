using DynamicData;
using LiteDB;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utility.Common.Helper;
using Utility.Helpers.Generic;
using Utility.Persists;
using Utility.Trees.Demo.Infrastructure;

namespace Utility.Trees.Demo.Two
{
    /// <summary>
    /// Interaction logic for ConnectionsUserControl.xaml
    /// </summary>
    public partial class ConnectionsView : UserControl
    {
        public static readonly DependencyProperty TreeViewProperty =
            DependencyProperty.Register("TreeView", typeof(TreeView), typeof(ConnectionsView), new PropertyMetadata(TreeViewChanged));

        public static readonly DependencyProperty ListBoxProperty =
            DependencyProperty.Register("ListBox", typeof(ListBox), typeof(ConnectionsView), new PropertyMetadata(ListBoxChanged));

        public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register("ViewModel", typeof(ConnectionsViewModel), typeof(ConnectionsView), new PropertyMetadata(ViewModelChanged));

        private readonly LiteDbRepository repository = new(new(typeof(ConnectionModel), nameof(ConnectionModel.Guid)));
        private readonly Converter converter;
        private readonly Dictionary<LineViewModel, LineViewModelInfo> dictionary = new();

        public ConnectionsView()
        {
            InitializeComponent();

            //this.ItemsControl.ItemsSource = Lines;
            MouseMove += ConnectionsUserControl_MouseMove;
            //var tree = new Tree(new ViewModel() { Name = "root" },
            //        new Tree(new ViewModel { Name = "A" },
            //        new Tree(new ViewModel { Name = "B" }),
            //        new Tree(new ViewModel { Name = "C" }),
            //        new Tree(new ViewModel { Name = "D" })));

            converter = new Converter(this);


            Grid.MouseRightButtonDown += Grid_MouseRightButtonDown;

        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in repository.All<ConnectionModel>())
            {
                ViewModel.Connections.Add(item);
                var line = converter.To(item);
                ViewModel.Lines.Add(line);
                dictionary.Add(line, new() { IsPersisted = true });
            }
        }

        private static void TreeViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is TreeView treeView && d is ConnectionsView connectionsView)
            {
                treeView.SelectedItemChanged += connectionsView.TreeView_SelectedItemChanged;
                treeView.PreviewMouseUp += connectionsView.TreeView_PreviewMouseUp;
   
                    connectionsView.WhenAnyValue(a => a.ViewModel).WhereNotNull().Subscribe(a =>
                    {
                        treeView.ItemsSource = a.ViewModel.Items;
                    });
                

                connectionsView.TreeContent.Content = treeView;
            }
        }

        private static void ListBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ListBox listBox && d is ConnectionsView connectionsView)
            {

                listBox.PreviewMouseUp += connectionsView.TreeView2_PreviewMouseUp;
                listBox.SelectionChanged += connectionsView.TreeView2_SelectedItemChanged;
                //listBox.ItemsSource = connectionsView.ViewModel.ServiceModel;
                connectionsView.WhenAnyValue(a => a.ViewModel).WhereNotNull().Subscribe(a =>
                {
                    listBox.ItemsSource = a.ServiceModel;
                });

                connectionsView.ListContent.Content = listBox;
            }
        }

        private static void ViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ConnectionsViewModel connectionsViewModel && d is ConnectionsView connectionsView)
            {
                connectionsView.DataContext = connectionsViewModel;
            }
        }

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.IsLine)
            {

                ViewModel.Lines.Remove(ViewModel.Last);
                ViewModel.Last = null;
                ViewModel.IsLine = false;
                ListBox.SelectedItem = null;
                TreeView.ClearSelections();
            }
        }

        public TreeView TreeView
        {
            get { return (TreeView)GetValue(TreeViewProperty); }
            set { SetValue(TreeViewProperty, value); }
        }

        public ListBox ListBox
        {
            get { return (ListBox)GetValue(ListBoxProperty); }
            set { SetValue(ListBoxProperty, value); }
        }

        public ConnectionsViewModel ViewModel
        {
            get { return (ConnectionsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
     ListBox.SelectedIndex = -1;

            if (e.NewValue == null)
                return;

            if (e.NewValue is not TreeViewItem { RenderSize: Size size } element)
            {
                element = TreeHelper.FindRecursive<TreeViewItem>(TreeView, e.NewValue) as TreeViewItem;
                size = element.RenderSize;
            }

            ViewModel.Point0 = new Point(element.RenderSize.Width, TreeView.FindDepth(element));

            if (ViewModel.Point1.HasValue && ViewModel.IsLine == false)
            {
                //ViewModel.Point0 = ViewModel.Point1 = null;
                //ViewModel.Last = null;

            }
            else
                Add();
        }

        private void TreeView2_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            TreeView.ClearSelections();

            if (e.AddedItems.Count == 0)
                return;
            if (e.AddedItems.Cast<object>().Single() is not FrameworkElement { RenderSize: var size } element)
            {
                element = ListBox.ItemContainerGenerator.ContainerFromItem(e.AddedItems.Cast<object>().Single()) as FrameworkElement;
                size = element.RenderSize;
            }


            Point ofs = new(0, size.Height / 2d);
            var pointA = ListBox.TransformToAncestor(this).Transform(ofs);
            var pointB = element.TransformToAncestor(ListBox).Transform(ofs);
            ViewModel.Point1 = new Point(pointA.X /*+ pointB.X*/, pointB.Y);
            if (ViewModel.Point0.HasValue && ViewModel.IsLine==false)
            {
                ViewModel.Point0 = ViewModel.Point1 = null;
                ViewModel.Last = null;
            }
            else
                Add();
        }

        private void ConnectionsUserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (ViewModel.Last != null)
            {
                // Perform the hit test against a given portion of the visual object tree.
                HitTestResult result = VisualTreeHelper.HitTest(this, e.GetPosition(this));
                var position = e.GetPosition(this); ;

                if (ViewModel.Point0 == null)
                {
                    if (position.X < Grid.ActualWidth - ListBox.ActualWidth)
                    {
                        ViewModel.IsLine = true;
                        if (ViewModel.Lines.Contains(ViewModel.Last) == false)
                            ViewModel.Lines.Add(ViewModel.Last);
                        ViewModel.Last.StartPoint = new Point(Math.Max(position.X, TreeView.ActualWidth), position.Y);

                    }

                    if (position.X > Grid.ActualWidth - ListBox.ActualWidth)
                    {
                        ViewModel.IsLine = false;
                        if (ViewModel.Lines.Contains(ViewModel.Last) == false)
                            ViewModel.Lines.Remove(ViewModel.Last);
                        ViewModel.Last.StartPoint = ViewModel.Last.EndPoint;
                    }
                }
                if (ViewModel.Point1 == null)
                {
                    if (position.X > TreeView.ActualWidth)
                    {
                        ViewModel.IsLine = true;
                        if (ViewModel.Lines.Contains(ViewModel.Last) == false)
                            ViewModel.Lines.Add(ViewModel.Last);
                        ViewModel.Last.EndPoint = new Point(Math.Min(position.X, Grid.ActualWidth - ListBox.ActualWidth), position.Y);
                    }

                    if (position.X < TreeView.ActualWidth)
                    {
                        ViewModel.IsLine = false;
                        if (ViewModel.Lines.Contains(ViewModel.Last) == false)
                            ViewModel.Lines.Remove(ViewModel.Last);
                        ViewModel.Last.EndPoint = ViewModel.Last.StartPoint; 
                    }
                }
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

            //for (var i = ViewModel.Lines.Count - 1; i >= 0; i--)
            //{
            if (ViewModel.IsLine)
            {
                var line = ViewModel.Last;
                if (dictionary.TryGetValue(line, out var persisted))
                {
                    if (persisted.IsPersisted == false)
                    {
                        if (converter.To(line) is ConnectionModel model)
                        {
                            repository.Add(model);
                            ViewModel.Connections.Add(model);
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
                        ViewModel.Connections.Add(model);
                        repository.Add(model);
                        dictionary.Add(line, new() { IsPersisted = true });
                    }
                    else
                    {
                        ViewModel.Lines.RemoveLast();
                    }
                }
            
   
                ViewModel.IsLine = false;
                ListBox.SelectedItem = null;
                TreeView.ClearSelections();
            }
        }

        void Add()
        {
            var pos = Mouse.GetPosition(this);

            ViewModel.Last = new LineViewModel { Guid = Guid.NewGuid(), StartPoint = ViewModel.Point0.HasValue ? ViewModel.Point0.Value : pos, EndPoint = ViewModel.Point1.HasValue ? ViewModel.Point1.Value : pos };
            //ViewModel.Lines.Add(ViewModel.Last);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Lines.Clear();
            ViewModel.Connections.Clear();
            repository.Clear();
        }

        private void Display_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}