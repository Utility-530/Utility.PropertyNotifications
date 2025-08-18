using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Utility.WPF.Demo.Panels;
using Utility.WPF.Panels.Infrastructure;

namespace Utility.WPF.Panels
{
    // Formerly CornerPanel
    public class EdgePanel : Panel
    {
        private const string MyRectangleName = "MyRectangle";
        private Size availableSize;
        private Size totalSize;
        private Size finalSize;
        private UIElement[] children;

        public static readonly DependencyProperty RegionProperty =
    DependencyProperty.Register("Region", typeof(CircleRegion), typeof(EdgePanel), new PropertyMetadata(CircleRegion.TopLeft));

        #region properties

        public CircleRegion Region
        {
            get { return (CircleRegion)GetValue(RegionProperty); }
            set { SetValue(RegionProperty, value); }
        }

        #endregion properties

        /// <summary>
        /// Measure the children
        /// </summary>
        /// <param name="availableSize">Size available</param>
        /// <returns>Size desired</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            children = this.Children.Cast<UIElement>().Where(a => !(a is FrameworkElement { Name: { } name } && name == MyRectangleName)).ToArray();

            foreach (var ch in this.Children.Cast<UIElement>().Where(a => (a is FrameworkElement { Name: { } name } && name == MyRectangleName)).ToArray())
            {
                ch.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            foreach (var child in children)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            var sizes = SelectSizes(children).ToArray();
            var count = SizerUtility.GetCount(sizes.Length);
            var array = MeasureHelper.GetRowsColumns2(availableSize, sizes, count, count);
            var sizer = SizerFactory.Create(circleRegion: CircleRegion.TopLeft);

            foreach (var (size, _) in array)
            {
                sizer.Append(size);
            }

            this.availableSize = availableSize;
            totalSize = sizer.GetTotalSize();
            return totalSize;
        }

        /// <summary>
        /// Arrange the children
        /// </summary>
        /// <param name="finalSize">Size available</param>
        /// <returns>Size used</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            children = this.Children.Cast<UIElement>().Where(a => !(a is FrameworkElement { Name: { } name } && name == MyRectangleName)).ToArray();

            if (children.Length > 0)
            {
                var region = Region;
                this.finalSize = finalSize;
                Point center = new Point(finalSize.Width / 2, finalSize.Height / 2);
                var sizer = SizerFactory.Create(circleRegion: region);
                var sizes = SelectSizes(children).ToArray();
                var count = SizerUtility.GetCount(sizes.Length);
                var array = MeasureHelper.GetRowsColumns2(finalSize, sizes, count, count);
                for (int i = 0; i < children.Length; i++)
                {
                    UIElement child = children[i];
                    var (childSize, _) = array[i];

                    var distanceFromCenter = GetDistance(region, new Vector(finalSize.Width - childSize.Width, finalSize.Height - childSize.Height));
                    var p = sizer.Append(childSize);
                    var offset = new Point(p.X + distanceFromCenter.X + center.X - childSize.Width / 2d, p.Y + distanceFromCenter.Y + center.Y - childSize.Height / 2d);
                    child.Arrange(new Rect(offset, childSize));
                }

                //return sizer.GetTotalSize();
            }

            foreach (var ch in this.Children.Cast<UIElement>().Where(a => (a is FrameworkElement { Name: { } name } && name == MyRectangleName)).ToArray())
            {
                ch.Arrange(new Rect(0, 0, availableSize.Width, availableSize.Height));
            }

            return finalSize;
        }

        private static IEnumerable<(double?, double?, UIElement element)> SelectSizes(IEnumerable<UIElement> elements)
        {
            return elements.Select(child =>
            {
                var heightSizing = EdgeLegacyPanel.GetHeightSizing(child);
                var widthSizing = EdgeLegacyPanel.GetWidthSizing(child);
                var width = (child.DesiredSize.Width == 0 || widthSizing == Sizing.FromParent) ? (double?)null : child.DesiredSize.Width;
                var height = (child.DesiredSize.Height == 0 || heightSizing == Sizing.FromParent) ? (double?)null : child.DesiredSize.Height;

                return (width, height, child);
            });
        }

        private static Point GetDistance(CircleRegion CircleRegion, Vector size)
        {
            return CircleRegion switch
            {
                CircleRegion.TopLeft => new Point(-size.X / 2, -size.Y / 2),
                CircleRegion.TopRight => new Point(size.X / 2, -size.Y / 2),
                CircleRegion.BottomRight => new Point(size.X / 2, size.Y / 2),
                CircleRegion.BottomLeft => new Point(-size.X / 2, size.Y / 2),

                CircleRegion.Top => new Point(0, -size.Y / 2),
                CircleRegion.Bottom => new Point(0, size.Y / 2),

                CircleRegion.Right => new Point(size.X / 2, 0),
                CircleRegion.Left => new Point(-size.X / 2, 0),

                CircleRegion.Middle => new Point(0, 0),
                _ => throw new NotImplementedException(),
            };
        }

        public void ShowLine()
        {
            foreach (var r in this.Children.OfType<FrameworkElement>().Where(a => a.Name == MyRectangleName).ToArray())
                this.Children.Remove(r);
            var rect = new Rectangle
            {
                Width = totalSize.Width,
                Height = totalSize.Height,
                Stroke = Brushes.LightSteelBlue,
                Name = MyRectangleName,
                StrokeThickness = 2
            };

            if (ItemsControl.GetItemsOwner(this) is { ItemsSource: IEnumerable source } itemsControl)
            {
                //if(typeof(IList<>).IsAssignableFrom(source.GetType()))
                // ((dynamic)source).Add(myLine);
                // Not possible if ItemsControl owns it
            }
            else
                this.Children.Add(rect);
        }
    }
}