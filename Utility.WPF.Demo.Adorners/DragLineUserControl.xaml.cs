using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utility.WPF.Attached;
using Utility.WPF.Helpers;

namespace Utility.WPF.Demo.Adorners
{
    /// <summary>
    /// Interaction logic for DragLineUserControl.xaml
    /// </summary>
    public partial class DragLineUserControl : UserControl
    {
        public DragLineUserControl()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(Grid);

            layer.AddIfMissingAdorner(new LineAdorner(Blue));

        }
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Blue.RemoveAdorners();
        }
    }


    public class LineAdorner : Adorner
    {
        private Point start;
        private Point end;
        //private Thumb startThumb;
        private Thumb endThumb;
        private Line selectedLine;
        private Ellipse ellipse;
        private VisualCollection visualChildren;
        private UIElement adornedElement;
        private Point position;
        private readonly Grid? grid;
        private readonly AdornerLayer _adornerLayer;

        // Constructor
        public LineAdorner(UIElement adornedElement) : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);

            //startThumb = new Thumb { Cursor = Cursors.Hand, Width = 10, Height = 10, Background = Brushes.Green };
            var element = (adornedElement as FrameworkElement);
            Size size = element.RenderSize;
            Point ofs = new(size.Width / 2, size.Height / 2d);
            var point = element.PointToScreen(new Point(0, 0));

            selectedLine = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                X1 = point.X + ofs.X,
                Y1 = point.Y + ofs.Y,
                X2 = point.X + ofs.X,
                Y2 = point.Y + ofs.Y
            };

            //endThumb = new Thumb { Cursor = Cursors.Hand, Width = 10, Height = 10, Background = Brushes.BlueViolet };
            ellipse = new Ellipse { Fill=Brushes.Purple, Width = 10, Height = 10, };

            //startThumb.DragDelta += StartDragDelta;
            //endThumb.DragDelta += EndDragDelta;

            visualChildren.Add(selectedLine);
            visualChildren.Add(ellipse);
            //visualChildren.Add(endThumb);

            this.adornedElement = adornedElement;
            grid = adornedElement.FindParent<Grid>();
            grid.PreviewDragOver += LineAdorner_PreviewDragOver; ;
            _adornerLayer = AdornerLayer.GetAdornerLayer(grid);
            grid.PreviewMouseMove += Grid_PreviewMouseMove;
        }

        private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            position = e.GetPosition(grid);

            _adornerLayer.Update(AdornedElement);
        }



        private void LineAdorner_PreviewDragOver(object sender, DragEventArgs e)
        {
            //position = e.GetPosition(grid);
            //_adornerLayer.Update(AdornedElement);

        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(position.X, position.Y));

            return result;
        }

        // Event for the Thumb Start Point
        //private void StartDragDelta(object sender, DragDeltaEventArgs e)
        //{
        //    Point position = Mouse.GetPosition(this);

        //    selectedLine.X1 = position.X;
        //    selectedLine.Y1 = position.Y;
        //}

        // Event for the Thumb End Point
        //private void EndDragDelta(object sender, DragDeltaEventArgs e)
        //{
        //    Point position = Mouse.GetPosition(this);
        //    UpdateDragAdorner(e.GetPosition(itemsControl));

        //    selectedLine.X2 = selectedLine.X2 + e.HorizontalChange;
        //    selectedLine.Y2 = selectedLine.Y2 + e.VerticalChange;
        //}

        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    if (AdornedElement is Line)
        //    {
        //        selectedLine = AdornedElement as Line;
        //        start = new Point(selectedLine.X1, selectedLine.Y1);
        //        end = new Point(selectedLine.X2, selectedLine.Y2);
        //    }
        //}

        protected override Size ArrangeOverride(Size finalSize)
        {
            //var minX = Math.Min(selectedLine.X1, selectedLine.X2);
            //var minY = Math.Min(selectedLine.Y1, selectedLine.Y2);
            //var diffX = Math.Abs(selectedLine.X1 - selectedLine.X2);
            //var diffY = Math.Abs(selectedLine.Y1 - selectedLine.Y2);
            //var startRect = new Rect(minX, minY, minX + diffX, minY + diffY);
            //selectedLine.Arrange(startRect);

            //var endRect = new Rect( - endThumb.Height / 2d, selectedLine.Y2 - endThumb.Width / 2d, endThumb.Width, endThumb.Height);
            //endThumb.Arrange(endRect);
            ellipse.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// Returns a <see cref="T:System.Windows.Media.Transform"/> for the adorner, based on the transform that is currently applied to the adorned element.
        /// </summary>
        /// <returns>
        /// A transform to apply to the adorner.
        /// </returns>
        /// <param name="transform">The transform that is currently applied to the adorned element.</param>
     



    }
}
