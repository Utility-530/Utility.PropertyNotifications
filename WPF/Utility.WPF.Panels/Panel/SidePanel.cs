namespace Utility.WPF.Demo.Panels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    public class SidePanel : Panel
    {
        private int rows;
        private int columns;
        private bool columnsChanged, rowsChanged;

        // Using a DependencyProperty as the backing store for Region.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegionProperty =
            DependencyProperty.Register("Region", typeof(CircleRegion), typeof(SidePanel), new PropertyMetadata(CircleRegion.Top));

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(SidePanel),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(int), typeof(SidePanel),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, Changed));

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SidePanel panel)
            {
                panel.rowsChanged |= e.Property == RowsProperty && (int)e.NewValue > 0;
                panel.columnsChanged |= e.Property == ColumnsProperty && (int)e.NewValue > 0;
            }
        }

        public CircleRegion Region
        {
            get { return (CircleRegion)GetValue(RegionProperty); }
            set { SetValue(RegionProperty, value); }
        }

        /// <summary>
        /// Get/Set the amount of columns this grid should have
        /// </summary>
        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// Get/Set the amount of rows this grid should have
        /// </summary>
        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        /// <summary>
        /// Measure the children
        /// </summary>
        /// <param name="availableSize">Size available</param>
        /// <returns>Size desired</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (!columnsChanged && !rowsChanged)
            {
                (columns, rows) = MeasureHelper.GetRowsColumns(availableSize, Children.Count);
            }
            else
            {
                columns = Math.Max(Columns, 1);
                rows = Math.Max(Rows, 1);
            }

            var fsd = new fsd();
            int i = 0;
            foreach (var child in InternalChildren.Cast<UIElement>())
            {
                child.Measure(availableSize);
                var individualSize = Region switch
                {
                    CircleRegion.Top => GetChildSize(child, availableSize, columns, rows),
                    CircleRegion.Bottom => GetChildSize(child, availableSize, columns, rows),
                    CircleRegion.Left => GetChildSize(child, availableSize, rows, columns),
                    CircleRegion.Right => GetChildSize(child, availableSize, rows, columns),
                };

                fsd.Append(individualSize, ChildSizer.GetCoordinate(i, columns));
                i++;
                child.Measure(individualSize);
            }
            //return fsd.TotalSize();

            return availableSize;
        }

        /// <summary>
        /// Get the size of the child element
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns>Returns the size of the child</returns>
        private static Size GetChildSize(UIElement child, Size availableSize, int columns, int rows)
        {
            var heightSizing = EdgeLegacyPanel.GetHeightSizing(child);
            var widthSizing = EdgeLegacyPanel.GetWidthSizing(child);
            var width = (child.DesiredSize.Width == 0 || widthSizing == Sizing.FromParent || child.DesiredSize.Width > availableSize.Width / columns) ?
                availableSize.Width / columns :
                child.DesiredSize.Width;
            var height = (child.DesiredSize.Height == 0 || heightSizing == Sizing.FromParent || child.DesiredSize.Height > availableSize.Height / rows) ?
                availableSize.Height / rows :
                child.DesiredSize.Height;

            return new Size(width, height);
        }

        /// <summary>
        /// Arrange the children
        /// </summary>
        /// <param name="finalSize">Size available</param>
        /// <returns>Size used</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var (child, (a, b, c)) in NewMethod(finalSize))
            {
                child.Arrange(c);
            }

            return finalSize;
        }

        private IEnumerable<(UIElement, (int, int, Rect))> NewMethod(Size finalSize)
        {
            var sdff = Region switch
            {
                CircleRegion.Top => new ChildSizer(rows, columns, finalSize, Region),
                CircleRegion.Bottom => new ChildSizer(rows, columns, finalSize, Region),
                CircleRegion.Left => new ChildSizer(columns, rows, finalSize, Region),
                CircleRegion.Right => new ChildSizer(columns, rows, finalSize, Region),
            };
            for (int i = 0; i < Children.Count; i++)
            {
                UIElement child = Children[i];
                var childSize = Region switch
                {
                    CircleRegion.Top => GetChildSize(child, finalSize, columns, rows),
                    CircleRegion.Bottom => GetChildSize(child, finalSize, columns, rows),
                    CircleRegion.Left => GetChildSize(child, finalSize, rows, columns),
                    CircleRegion.Right => GetChildSize(child, finalSize, rows, columns),
                };
                yield return (child, sdff.GetChildRect(i, childSize));
            }
        }

        private class ChildSizer
        {
            private readonly int columns;
            private readonly int rows;
            private readonly Size totalSize;
            private readonly CircleRegion region;

            public ChildSizer(int columns, int rows, Size totalSize, CircleRegion region)
            {
                this.columns = columns;
                this.rows = rows;
                this.totalSize = totalSize;
                this.region = region;
            }

            /// <summary>
            /// Arrange the individual children
            /// </summary>
            /// <param name="index"></param>
            /// <param name="child"></param>
            /// <param name="finalSize"></param>
            public (int, int, Rect) GetChildRect(int index, Size childSize)
            {
                return GetChildRect(GetCoordinate(index, columns), childSize, totalSize, rows, columns, region);
            }

            public static Coordinate GetCoordinate(int index, int columns)
            {
                int row = index / columns;
                int column = (columns / 2 + (index % 2 * 2 - 1) * ((index % columns) + 1) / 2) - ((1 + columns) % 2);
                Debug.WriteLine("row:" + row + " column:" + column + " index:" + index);
                return new Coordinate(row, column);
            }

            /// <summary>
            /// Arrange the individual children
            /// </summary>
            /// <param name="index"></param>
            /// <param name="child"></param>
            /// <param name="finalSize"></param>
            public static (int, int, Rect) GetChildRect(Coordinate coordinate, Size childSize, Size totalSize, int rows, int columns, CircleRegion region)
            {
                var (row, column) = region switch
                {
                    CircleRegion.Left => (coordinate.Y, coordinate.X),
                    CircleRegion.Right => (coordinate.Y, coordinate.X),
                    CircleRegion.Top => (coordinate.X, coordinate.Y),
                    CircleRegion.Bottom => (coordinate.X, coordinate.Y),
                };

                Point center = new Point(totalSize.Width / 2, totalSize.Height / 2);

                var offSetFactor = ((columns) / 2d);

                var (xPosition, yPosition) = region switch
                {
                    CircleRegion.Left => (column * childSize.Width, center.Y + (row - offSetFactor) * childSize.Height),
                    CircleRegion.Right => (totalSize.Width - ((column + 1) * childSize.Width), center.Y + (row - offSetFactor) * childSize.Height),
                    CircleRegion.Top => (center.X + (column - offSetFactor) * childSize.Width, row * childSize.Height),
                    CircleRegion.Bottom => (center.X + (column - offSetFactor) * childSize.Width, totalSize.Height - ((row + 1) * childSize.Height)),
                };

                // Debug.WriteLine("x:" + xPosition + distance.X + " y:" + yPosition + distance.Y);

                return (row, column, new Rect(
                    xPosition,
                    yPosition, childSize.Width, childSize.Height));
            }
        }

        private class fsd
        {
            private Dictionary<int, double> widthSizes = new Dictionary<int, double>();
            private Dictionary<int, double> heightSizes = new Dictionary<int, double>();

            public fsd()
            {
            }

            /// <summary>
            /// Get the size of the child element
            /// </summary>
            /// <param name="availableSize"></param>
            /// <returns>Returns the size of the child</returns>
            public Size Append(Size childSize, int column, int row)
            {
                var maxWidth = heightSizes[column] = Math.Max(widthSizes.GetValueOrDefault(column), childSize.Width);
                var maxHeight = widthSizes[row] = Math.Max(widthSizes.GetValueOrDefault(row), childSize.Width);
                return new Size(maxWidth, maxHeight);
            }

            public Size Append(Size childSize, Coordinate coordinate)
            {
                var maxWidth = heightSizes[coordinate.Y] = Math.Max(widthSizes.GetValueOrDefault(coordinate.Y), childSize.Width);
                var maxHeight = widthSizes[coordinate.X] = Math.Max(widthSizes.GetValueOrDefault(coordinate.X), childSize.Width);
                return new Size(maxWidth, maxHeight);
            }

            public Size TotalSize()
            {
                return new Size(widthSizes.Sum(a => a.Value), heightSizes.Sum(a => a.Value));
            }
        }
    }
}

//namespace Utility.WPF.Demo.Panels
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Diagnostics;
//    using System.Linq;
//    using System.Windows;
//    using System.Windows.Controls;

//    public class Side2Panel : Panel
//    {
//        private int rows;
//        private int columns;
//        bool columnsChanged, rowsChanged;

//        // Using a DependencyProperty as the backing store for Region.  This enables animation, styling, binding, etc...
//        public static readonly DependencyProperty RegionProperty =
//            DependencyProperty.Register("Region", typeof(CircleRegion), typeof(Side2Panel), new PropertyMetadata(CircleRegion.Top));

//        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(Side2Panel),
//            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

//        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(int), typeof(Side2Panel),
//            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, Changed));

//        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            if (d is Side2Panel panel)
//            {
//                panel.rowsChanged |= e.Property == RowsProperty && (int)e.NewValue > 0;
//                panel.columnsChanged |= e.Property == ColumnsProperty && (int)e.NewValue > 0;
//            }
//        }

//        public CircleRegion Region
//        {
//            get { return (CircleRegion)GetValue(RegionProperty); }
//            set { SetValue(RegionProperty, value); }
//        }

//        /// <summary>
//        /// Get/Set the amount of columns this grid should have
//        /// </summary>
//        public int Columns
//        {
//            get { return (int)GetValue(ColumnsProperty); }
//            set { SetValue(ColumnsProperty, value); }
//        }

//        /// <summary>
//        /// Get/Set the amount of rows this grid should have
//        /// </summary>
//        public int Rows
//        {
//            get { return (int)GetValue(RowsProperty); }
//            set { SetValue(RowsProperty, value); }
//        }

//        /// <summary>
//        /// Measure the children
//        /// </summary>
//        /// <param name="availableSize">Size available</param>
//        /// <returns>Size desired</returns>
//        protected override Size MeasureOverride(Size availableSize)
//        {
//            if (!columnsChanged && !rowsChanged)
//            {
//                (columns, rows) = MeasureHelper.GetRowsColumns(availableSize, Children.Count);
//            }
//            else
//            {
//                columns = Math.Max(Columns, 1);
//                rows = Math.Max(Rows, 1);
//            }

//            var dsf = new fsd();
//            int i = 0;
//            foreach (var child in InternalChildren.Cast<UIElement>())
//            {
//                i++;
//                child.Measure(new Size(availableSize.Width, availableSize.Height));
//                dsf.Append(child.DesiredSize, ChildSizer.GetCoordinate(i, columns));

//            }
//            return availableSize;
//            //return dsf.TotalSize();
//        }

//        /// <summary>
//        /// Arrange the children
//        /// </summary>
//        /// <param name="finalSize">Size available</param>
//        /// <returns>Size used</returns>
//        protected override Size ArrangeOverride(Size finalSize)
//        {
//            foreach (var (child, (a, b, c)) in NewMethod(finalSize))
//            {
//                child.Arrange(c);
//            }

//            return finalSize;

//        }

//        private IEnumerable<(UIElement, (int, int, Rect))> NewMethod(Size finalSize)
//        {
//            var sdff = Region switch
//            {
//                CircleRegion.Top => new ChildSizer(rows, columns, finalSize, Region),
//                CircleRegion.Bottom => new ChildSizer(rows, columns, finalSize, Region),
//                CircleRegion.Left => new ChildSizer(columns, rows, finalSize, Region),
//                CircleRegion.Right => new ChildSizer(columns, rows, finalSize, Region),
//            };
//            for (int i = 0; i < Children.Count; i++)
//            {
//                UIElement child = Children[i];
//                yield return (child, sdff.GetChildRect(i, child.DesiredSize));
//            }
//        }

//        class ChildSizer
//        {
//            private readonly int columns;
//            private readonly int rows;
//            private readonly Size totalSize;
//            private readonly CircleRegion region;

//            public ChildSizer(int columns, int rows, Size totalSize, CircleRegion region)
//            {
//                this.columns = columns;
//                this.rows = rows;
//                this.totalSize = totalSize;
//                this.region = region;
//            }

//            /// <summary>
//            /// Arrange the individual children
//            /// </summary>
//            /// <param name="index"></param>
//            /// <param name="child"></param>
//            /// <param name="finalSize"></param>
//            public (int, int, Rect) GetChildRect(int index, Size childSize)
//            {
//                return GetChildRect(GetCoordinate(index, columns), childSize, totalSize, rows, columns, region);
//            }

//            public static Coordinate GetCoordinate(int index, int columns)
//            {
//                int row = index / columns;
//                int column = (columns / 2 + (index % 2 * 2 - 1) * ((index % columns) + 1) / 2) - ((1 + columns) % 2);
//                Debug.WriteLine("row:" + row + " column:" + column + " index:" + index);
//                return new Coordinate(row, column);
//            }

//            /// <summary>
//            /// Arrange the individual children
//            /// </summary>
//            /// <param name="index"></param>
//            /// <param name="child"></param>
//            /// <param name="finalSize"></param>
//            public static (int, int, Rect) GetChildRect(Coordinate coordinate, Size childSize, Size totalSize, int rows, int columns, CircleRegion region)
//            {
//                var (row, column) = region switch
//                {
//                    CircleRegion.Left => (coordinate.Y, coordinate.X),
//                    CircleRegion.Right => (coordinate.Y, coordinate.X),
//                    CircleRegion.Top => (coordinate.X, coordinate.Y),
//                    CircleRegion.Bottom => (coordinate.X, coordinate.Y),

//                };

//                int currentPage;
//                Point center = new Point(totalSize.Width / 2, totalSize.Height / 2);

//                var offSetFactor =  ((columns) / 2d);

//                var (xPosition, yPosition) = region switch
//                {
//                    CircleRegion.Left => (column * childSize.Width, center.Y + (row - offSetFactor) * childSize.Height),
//                    CircleRegion.Right => (totalSize.Width - ((column + 1) * childSize.Width), center.Y + (row - offSetFactor) * childSize.Height),
//                    CircleRegion.Top => (center.X + (column - offSetFactor) * childSize.Width, row * childSize.Height),
//                    CircleRegion.Bottom => (center.X + (column - offSetFactor) * childSize.Width, totalSize.Height - ((row + 1) * childSize.Height)),
//                };

//                // Debug.WriteLine("x:" + xPosition + distance.X + " y:" + yPosition + distance.Y);

//                return (row, column, new Rect(
//                    xPosition,
//                    yPosition, childSize.Width, childSize.Height));
//            }
//        }

//        static Point GetDistance(CircleRegion CircleRegion, Vector size)
//        {
//            return CircleRegion switch
//            {
//                CircleRegion.Top => new Point(0, 0),
//                CircleRegion.Bottom => new Point(0, -size.Y),

//                CircleRegion.Right => new Point(-size.X, 0),
//                CircleRegion.Left => new Point(0, 0),

//                CircleRegion.Middle => new Point(0, 0),
//                _ => throw new NotImplementedException(),
//            };
//        }

//        class fsd
//        {
//            Dictionary<int, double> widthSizes = new Dictionary<int, double>();
//            Dictionary<int, double> heightSizes = new Dictionary<int, double>();

//            public fsd()
//            {
//            }

//            /// <summary>
//            /// Get the size of the child element
//            /// </summary>
//            /// <param name="availableSize"></param>
//            /// <returns>Returns the size of the child</returns>
//            public Size Append(Size childSize, int column, int row)
//            {
//                var maxWidth = heightSizes[column] = Math.Max(widthSizes.GetValueOrDefault(column), childSize.Width);
//                var maxHeight = widthSizes[row] = Math.Max(widthSizes.GetValueOrDefault(row), childSize.Width);
//                return new Size(maxWidth, maxHeight);
//            }

//            public Size Append(Size childSize, Coordinate coordinate)
//            {
//                var maxWidth = heightSizes[coordinate.Y] = Math.Max(widthSizes.GetValueOrDefault(coordinate.Y), childSize.Width);
//                var maxHeight = widthSizes[coordinate.X] = Math.Max(widthSizes.GetValueOrDefault(coordinate.X), childSize.Width);
//                return new Size(maxWidth, maxHeight);
//            }

//            public Size TotalSize()
//            {
//                return new Size(widthSizes.Sum(a => a.Value), heightSizes.Sum(a => a.Value));
//            }
//        }
//    }
//}