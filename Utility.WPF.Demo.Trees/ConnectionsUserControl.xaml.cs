using Jellyfish;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Demo.Trees
{
    /// <summary>
    /// Interaction logic for ConnectionsUserControl.xaml
    /// </summary>
    public partial class ConnectionsUserControl : UserControl
    {
        ObservableCollection<LineViewModel> Lines { get; } = new();

        public ConnectionsUserControl()
        {
            InitializeComponent();
            this.ItemsControl.ItemsSource = Lines;
        }

        Point? point0, point1;

        private void MyTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var element = (e.NewValue as FrameworkElement);
            Size size = element.RenderSize;
            Point ofs = new(size.Width, size.Height / 2d);
            point0 = element.TransformToAncestor(MyTreeView).Transform(ofs);
            if (point1.HasValue)
            {
                Add();
            }
        }

        private void MyTreeView2_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var element = (e.NewValue as FrameworkElement);
            Size size = element.RenderSize;
            Point ofs = new(0, size.Height / 2d);
            point1 = element.TransformToAncestor(MyTreeView2).Transform(ofs);
            var x = MyTreeView2.TransformToAncestor(Grid).Transform(point1.Value);
            point1 = x;

            if (point0.HasValue)
            {
                Add();
            }
       
        }

        void Add()
        {

            Lines.Add(new LineViewModel { StartPoint = point0.Value, EndPoint = point1.Value });
            point0 = point1 = null;

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
