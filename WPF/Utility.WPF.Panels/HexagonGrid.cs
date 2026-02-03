using System;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Panels
{
    public class HexagonGrid : Grid
    {
        public static readonly DependencyProperty IsArrangeAutomaticProperty =
      DependencyProperty.Register(nameof(IsArrangeAutomatic), typeof(bool), typeof(HexagonGrid), new PropertyMetadata(false));

        public static readonly DependencyProperty ItemLengthProperty =
    DependencyProperty.Register("ItemLength", typeof(double), typeof(HexagonGrid),
        new FrameworkPropertyMetadata((double)0,
            FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty SpaceLengthProperty =
    DependencyProperty.Register("SpaceLength", typeof(double), typeof(HexagonGrid),
        new FrameworkPropertyMetadata((double)0,
            FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty RowsProperty =
    DependencyProperty.Register("Rows", typeof(int), typeof(HexagonGrid),
        new FrameworkPropertyMetadata(-1,
            FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(HexagonGrid),
                new FrameworkPropertyMetadata(-1,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));
        /**
         * If the length of 1 side of the hexagon = S, then:
         * Width = 2 x S
         * Height = S x SQRT(3)
         * Column C starts at C x (0.75 x Width)
         * Row R starts at R x Height
         * A row's uneven columns have an vertical offset of 0.5 x Height
         **/
        #region properties
        /// <summary>
        /// represents the length of 1 side of the hexagon.
        /// </summary>
        public double ItemLength
        {
            get { return (double)GetValue(ItemLengthProperty); }
            set { SetValue(ItemLengthProperty, value); }
        }
        /// <summary>
        /// represents the length of 1 side of the hexagon.
        /// </summary>
        public double SpaceLength
        {
            get { return (double)GetValue(SpaceLengthProperty); }
            set { SetValue(SpaceLengthProperty, value); }
        }

        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public bool IsArrangeAutomatic
        {
            get { return (bool)GetValue(IsArrangeAutomaticProperty); }
            set { SetValue(IsArrangeAutomaticProperty, value); }
        }
        #endregion properties

        protected override Size MeasureOverride(Size constraint)
        {
            double side = SpaceLength;
            double width = 2 * side;
            double height = side * Math.Sqrt(3.0);
            double colWidth = 0.75 * width;
            double rowHeight = height;

            Size availableChildSize = new Size(width, height);

            foreach (FrameworkElement child in InternalChildren)
            {
                child.Measure(availableChildSize);
            }

            int actualRows = Rows;
            int actualCols = Columns;

            // Auto-calculate if both are -1
            if (Columns == -1)
            {
                actualCols = (int)Math.Ceiling(Math.Sqrt(InternalChildren.Count));
            }
            if (Rows == -1)
            {
                actualRows = (int)Math.Ceiling((double)InternalChildren.Count / actualCols);
            }


            double totalHeight = actualRows * rowHeight;
            if (actualCols > 1)
                totalHeight += 0.5 * rowHeight;
            double totalWidth = actualCols + 0.5 * side;

            Size totalSize = new Size(totalWidth, totalHeight);

            return totalSize;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double side = SpaceLength;
            double width = 2 * side;
            double height = side * Math.Sqrt(3.0);
            double colWidth = 0.75 * width;
            double rowHeight = height;

            var _childSize = childSize();
            int actualRows = Rows;
            int actualCols = Columns;

            // Auto-calculate if both are -1
            if (Columns == -1)
            {
                actualCols = (int)Math.Ceiling(Math.Sqrt(InternalChildren.Count));
            }
            if (Rows == -1)
            {
                actualRows = (int)Math.Ceiling((double)InternalChildren.Count / actualCols);
            }

            foreach (FrameworkElement child in InternalChildren)
            {
                int row, col;
                if (IsArrangeAutomatic)
                {
                    int index = InternalChildren.IndexOf(child);
                    row = index / actualCols;
                    col = index % actualCols;
                    SetRow(child, row);
                    SetColumn(child, col);
                }
                else
                {
                    row = GetRow(child);
                    col = GetColumn(child);
                }
                double left = col * colWidth;
                double top = row * rowHeight;
                bool isUnevenCol = col % 2 != 0;
                if (isUnevenCol)
                    top += 0.5 * rowHeight;

                child.Arrange(new Rect(new Point(left, top), _childSize));
            }

            return arrangeSize;

            Size childSize()
            {
                double side = ItemLength;
                double width = 2 * side;
                double height = side * Math.Sqrt(3.0);
                double colWidth = 0.75 * width;
                double rowHeight = height;

                return new Size(width, height);
            }
        }
    }
}