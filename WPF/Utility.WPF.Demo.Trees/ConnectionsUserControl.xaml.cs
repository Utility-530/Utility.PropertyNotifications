using Jellyfish;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Utility.WPF.Demo.Trees
{
    /// <summary>
    /// Interaction logic for ConnectionsUserControl.xaml
    /// </summary>
    public partial class ConnectionsUserControl : UserControl
    {
        private Point? point0, point1;
        private LineViewModel last;
        private ObservableCollection<LineViewModel> Lines { get; } = new();

        public ConnectionsUserControl()
        {
            InitializeComponent();
            this.ItemsControl.ItemsSource = Lines;
            MouseMove += ConnectionsUserControl_MouseMove;
        }

   
        private void MyTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var element = (e.NewValue as FrameworkElement);
            Size size = element.RenderSize;
            Point ofs = new(size.Width, size.Height / 2d);
            point0 = element.TransformToAncestor(MyTreeView).Transform(ofs);
            if (point1.HasValue)
            {
                point0 = point1 = null;
                last = null;

            }
            else
                Add();
        }

        private void ConnectionsUserControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (last != null)
            {
                // Perform the hit test against a given portion of the visual object tree.
                HitTestResult result = VisualTreeHelper.HitTest(this, e.GetPosition(this));
                var position = e.GetPosition(this); ;

                if (point0 == null)
                {
                    last.StartPoint = new Point(Math.Max(position.X, MyTreeView.ActualWidth), position.Y);
                }
                if (point1 == null)
                    last.EndPoint = new Point(Math.Min(position.X, MyTreeView.ActualWidth + MiddleTreeView.ActualWidth), position.Y);
            } 
        }

        private void MyTreeView2_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var element = (e.NewValue as FrameworkElement);
            Size size = element.RenderSize;
            Point ofs = new(0, size.Height / 2d);
            var pointA = MyTreeView2.TransformToAncestor(this).Transform(ofs);
            var pointB = element.TransformToAncestor(MyTreeView2).Transform(ofs);
            point1 = new Point(pointA.X /*+ pointB.X*/,  pointB.Y);
            if (point0.HasValue)
            {
                point0 = point1 = null;
                last = null;
            }
            else
                Add();
        }

        Point GetMousePosition() => this.PointToScreen(Mouse.GetPosition(this));

        private void MyTreeView2_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem treeViewItem)
            {
                treeViewItem.IsSelected = true;
            }
        }

        private void MyTreeView1_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        void Add()
        {

            var pos = Mouse.GetPosition(this);
            Lines.Add(last = new LineViewModel { StartPoint = point0.HasValue ? point0.Value : pos, EndPoint = point1.HasValue ? point1.Value : pos });

        }
    }


    public class LineViewModel : ViewModel
    {
        private Point _startPoint;
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
}
