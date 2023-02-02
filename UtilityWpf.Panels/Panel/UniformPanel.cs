namespace Auxide.Controls
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    public class UniformPanel : Panel
    {
        private int rows;
        private int columns;
        private bool columnsChanged, rowsChanged;

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(UniformPanel),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(int), typeof(UniformPanel),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, Changed));

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UniformPanel panel)
            {
                panel.rowsChanged |= e.Property == RowsProperty && ((int)e.NewValue > 0);
                panel.columnsChanged |= e.Property == ColumnsProperty && ((int)e.NewValue > 0);
            }
        }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.RegisterAttached("Orientation", typeof(Orientation), typeof(UniformPanel),
            new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Get/Set the amount of columns this grid should have
        /// </summary>
        public int Columns
        {
            get { return (int)this.GetValue(ColumnsProperty); }
            set { this.SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// Get/Set the amount of rows this grid should have
        /// </summary>
        public int Rows
        {
            get { return (int)this.GetValue(RowsProperty); }
            set { this.SetValue(RowsProperty, value); }
        }

        /// <summary>
        /// Get/Set the orientation of the panel
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
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
                (rows, columns) = UtilityWpf.Demo.Panels.MeasureHelper.GetRowsColumns(availableSize, this.Children.Count);
            }
            else
            {
                columns = Math.Max(Columns, 1);
                rows = Math.Max(Rows, 1);
            }

            var individualSize = GetChildSize(availableSize, columns, rows);

            foreach (var child in InternalChildren.Cast<UIElement>())
            {
                child.Measure(individualSize);
            }
            if (double.IsInfinity(availableSize.Height) || double.IsInfinity(availableSize.Width))
            {
                return new Size(0, 0);
            }
            return availableSize;
        }

        /// <summary>
        /// Arrange the children
        /// </summary>
        /// <param name="finalSize">Size available</param>
        /// <returns>Size used</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size childSize = GetChildSize(finalSize, columns, rows);

            for (int i = 0; i < this.Children.Count; i++)
            {
                UIElement child = this.Children[i];

                child.Arrange(GetChildRect(i, childSize, columns, rows, Orientation));
            }

            return finalSize;

            /// <summary>
            /// Arrange the individual children
            /// </summary>
            /// <param name="index"></param>
            /// <param name="child"></param>
            /// <param name="finalSize"></param>
            static Rect GetChildRect(int index, Size childSize, int columns, int rows, Orientation orientation)
            {
                int row = index / columns;
                int column = index % columns;

                double xPosition, yPosition;

                int currentPage;

                if (orientation == Orientation.Horizontal)
                {
                    currentPage = (int)Math.Floor((double)index / (columns * rows));

                    xPosition = (currentPage) + (column * childSize.Width);
                    yPosition = (row % rows) * childSize.Height;
                }
                else
                {
                    xPosition = (column * childSize.Width);
                    yPosition = (row * childSize.Height);
                }

                return new Rect(xPosition, yPosition, childSize.Width, childSize.Height);
            }
        }

        /// <summary>
        /// Get the size of the child element
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns>Returns the size of the child</returns>
        private static Size GetChildSize(Size availableSize, int columns, int rows)
        {
            double width = availableSize.Width / columns;
            double height = availableSize.Height / rows;

            return new Size(width, height);
        }
    }
}