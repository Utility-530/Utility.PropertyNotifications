using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Tiny.Toolkits;

namespace Utility.WPF.Adorners
{
    /// <summary>
    /// An adorner that positions itself where the last item of an ItemsControl would be
    /// if there were one more item in the collection.
    /// </summary>
    public class LastItemAdorner : Adorner
    {
        enum VisualState
        {
            Pressed,
            Hovered,
            Normal
        }
        private readonly ItemsControl _itemsControl;
        private readonly Action content;
        private VisualState currentState;
        private ItemsPresenter itemsPresenter;

        public LastItemAdorner(ItemsControl adornedElement, Action? content = null)
            : base(adornedElement)
        {
            _itemsControl = adornedElement ?? throw new ArgumentNullException(nameof(adornedElement));
            this.content = content;

            // Add the content as a visual child
            //AddVisualChild(_content);
            //AddLogicalChild(_content);
        }



        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(LastItemAdorner), new PropertyMetadata());



        //protected override Visual GetVisualChild(int index)
        //{
        //    if (index != 0)
        //        throw new ArgumentOutOfRangeException(nameof(index));
        //    return _content;
        //}

        //protected override Size MeasureOverride(Size constraint)
        //{
        //    _content.Measure(constraint);
        //    return _content.DesiredSize;
        //}

        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    Point position = CalculateLastItemPosition();
        //    Size contentSize = _content.DesiredSize;

        //    _content.Arrange(new Rect(position, contentSize));
        //    return finalSize;
        //}

        private void EllipseAdorner_MouseDown(object sender, MouseButtonEventArgs e)
        {
            content?.Invoke();
            Command?.Execute(this);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (currentState == VisualState.Normal)
            {
                currentState = VisualState.Hovered;
                InvalidateVisual();
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            currentState = VisualState.Normal;
            InvalidateVisual();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            currentState = VisualState.Pressed;
            CaptureMouse();
            InvalidateVisual();
            e.Handled = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (currentState == VisualState.Pressed)
            {
                if (IsMouseOver)
                {
                    // Fire click event
                    content?.Invoke();
                    Command?.Execute(this);
                    if (IsMouseOver)
                        currentState = VisualState.Hovered;
                    else

                        currentState = VisualState.Normal;
                }
                else
                {
                    currentState = VisualState.Normal;
                }
            }

            ReleaseMouseCapture();
            InvalidateVisual();
            e.Handled = true;
        }



        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }


        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(int), typeof(LastItemAdorner), new PropertyMetadata(10));



        protected override void OnRender(DrawingContext drawingContext)
        {
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.WhiteSmoke);

            Pen renderPen = new Pen(new SolidColorBrush(
                currentState
                switch
                {
                    VisualState.Pressed => Colors.DarkMagenta,
                    VisualState.Normal => Colors.DarkSlateGray,
                    VisualState.Hovered => Colors.DarkOrange
                }), 2.0);


            if (AdornedElement is FrameworkElement control)
            {
                Point center = CalculateLastItemPosition();
                center = new Point(center.X, center.Y + Size / 2);
                // Draw the ellipse
                drawingContext.DrawEllipse(renderBrush, renderPen, center, Size, Size);

                // Draw the plus symbol inside the ellipse
                //Point center = new(control.Width, control.Height);

                double plusSize = 0.6 * Size; // Adjust size to fit nicely within the 10-radius ellipse

                // Draw horizontal line of the plus
                drawingContext.DrawLine(renderPen,
                    new Point(center.X - plusSize, center.Y),
                    new Point(center.X + plusSize, center.Y));

                // Draw vertical line of the plus
                drawingContext.DrawLine(renderPen,
                    new Point(center.X, center.Y - plusSize),
                    new Point(center.X, center.Y + plusSize));

            }
        }

        private Point CalculateLastItemPosition()
        {
            itemsPresenter = FindItemsPresenter(_itemsControl);
            if (itemsPresenter == null)
                return new Point(0, 0);

            var panel = FindItemsPanel(itemsPresenter);
            if (panel == null)
                return new Point(0, 0);

            return CalculatePositionBasedOnPanel(panel);
        }

        private Point CalculatePositionBasedOnPanel(Panel panel)
        {
            switch (panel)
            {
                case StackPanel stackPanel:
                    return CalculateStackPanelPosition(stackPanel);

                case WrapPanel wrapPanel:
                    return CalculateWrapPanelPosition(wrapPanel);

                case UniformGrid uniformGrid:
                    return CalculateUniformGridPosition(uniformGrid);

                case Grid grid:
                    return CalculateGridPosition(grid);

                case VirtualizingStackPanel virtualizingStackPanel:
                    return CalculateVirtualizingStackPanelPosition(virtualizingStackPanel);

                default:
                    // For custom panels, try to position at the end
                    return CalculateGenericPanelPosition(panel);
            }
        }

        private Point CalculateStackPanelPosition(StackPanel stackPanel)
        {
            if (stackPanel.Children.Count == 0)
                return new Point(0, 0);

            var lastChild = stackPanel.Children[stackPanel.Children.Count - 1] as FrameworkElement;

            if (stackPanel.Orientation == Orientation.Vertical)
            {
                // Get the visual bounds of all items to find the actual content area
                var itemsContainer = stackPanel;
                var lastItemContainer = (lastChild.FindParent<ContentPresenter>() ?? lastChild) as FrameworkElement;

                var itemsControl = itemsPresenter;
                if (itemsControl != null)
                {
                    var point = lastChild.TranslatePoint(new Point(0, lastChild.ActualHeight), itemsControl);
                    double y = point.Y ;
                    if (lastChild is FrameworkElement fe)
                        y += fe.Margin.Bottom;

                    return new Point(itemsControl.ActualWidth / 2d, y);
                }
                throw new InvalidOperationException("Could not find parent ItemsControl.");
                //var bounds = transform.TransformBounds(new Rect(0, 0,
                //    lastItemContainer.ActualWidth,
                //    lastItemContainer.ActualHeight));

                //double y = bounds.Bottom + (lastChild as FrameworkElement)?.Margin.Bottom ?? 0;
                //double x = bounds.Left + (bounds.Width / 2d); // Center on the item itself

                //return new Point(x, y);
            }
            else
            {
                double x = lastChild.TranslatePoint(new Point(lastChild.RenderSize.Width, 0), stackPanel).X;
                return new Point(x, 0);
            }
        }

        private Point CalculateWrapPanelPosition(WrapPanel wrapPanel)
        {
            if (wrapPanel.Children.Count == 0)
                return new Point(0, 0);

            var lastChild = wrapPanel.Children[wrapPanel.Children.Count - 1];
            var lastChildPosition = lastChild.TranslatePoint(new Point(0, 0), wrapPanel);

            if (wrapPanel.Orientation == Orientation.Horizontal)
            {
                double nextX = lastChildPosition.X + lastChild.RenderSize.Width;

                // Check if we need to wrap to next line
                //if (nextX + _content.DesiredSize.Width > wrapPanel.ActualWidth)
                //{
                //    return new Point(0, lastChildPosition.Y + lastChild.RenderSize.Height);
                //}
                return new Point(nextX, lastChildPosition.Y);
            }
            else
            {
                double nextY = lastChildPosition.Y + lastChild.RenderSize.Height;

                // Check if we need to wrap to next column
                //if (nextY + _content.DesiredSize.Height > wrapPanel.ActualHeight)
                //{
                //    return new Point(lastChildPosition.X + lastChild.RenderSize.Width, 0);
                //}
                return new Point(lastChildPosition.X, nextY);
            }
        }

        private Point CalculateUniformGridPosition(UniformGrid uniformGrid)
        {
            int itemCount = uniformGrid.Children.Count;
            int columns = uniformGrid.Columns > 0 ? uniformGrid.Columns :
                         (int)Math.Ceiling(Math.Sqrt(itemCount));
            int rows = uniformGrid.Rows > 0 ? uniformGrid.Rows :
                      (int)Math.Ceiling((double)itemCount / columns);

            int nextRow = itemCount / columns;
            int nextCol = itemCount % columns;

            double cellWidth = uniformGrid.ActualWidth / columns;
            double cellHeight = uniformGrid.ActualHeight / rows;

            return new Point(nextCol * cellWidth, nextRow * cellHeight);
        }

        private Point CalculateGridPosition(Grid grid)
        {
            // For Grid, position at 0,0 of the next available cell
            // This is a simplified approach - you might want to customize based on your needs
            if (grid.Children.Count == 0)
                return new Point(0, 0);

            // Find the next available grid position
            int maxRow = 0, maxCol = 0;
            foreach (UIElement child in grid.Children)
            {
                int row = Grid.GetRow(child);
                int col = Grid.GetColumn(child);
                maxRow = Math.Max(maxRow, row);
                maxCol = Math.Max(maxCol, col);
            }

            // Position in the next column of the last row, or next row if at end
            if (maxCol < grid.ColumnDefinitions.Count - 1)
            {
                return GetGridCellPosition(grid, maxRow, maxCol + 1);
            }
            else
            {
                return GetGridCellPosition(grid, maxRow + 1, 0);
            }
        }

        private Point GetGridCellPosition(Grid grid, int row, int col)
        {
            double x = 0, y = 0;

            for (int i = 0; i < col && i < grid.ColumnDefinitions.Count; i++)
            {
                x += grid.ColumnDefinitions[i].ActualWidth;
            }

            for (int i = 0; i < row && i < grid.RowDefinitions.Count; i++)
            {
                y += grid.RowDefinitions[i].ActualHeight;
            }

            return new Point(x, y);
        }

        private Point CalculateVirtualizingStackPanelPosition(VirtualizingStackPanel virtualizingStackPanel)
        {
            // Similar to StackPanel but account for virtualization
            if (virtualizingStackPanel.Children.Count == 0)
                return new Point(0, 0);

            var lastChild = virtualizingStackPanel.Children[virtualizingStackPanel.Children.Count - 1];

            if (virtualizingStackPanel.Orientation == Orientation.Vertical)
            {
                double y = lastChild.TranslatePoint(new Point(0, lastChild.RenderSize.Height), virtualizingStackPanel).Y;
                return new Point(0, y);
            }
            else
            {
                double x = lastChild.TranslatePoint(new Point(lastChild.RenderSize.Width, 0), virtualizingStackPanel).X;
                return new Point(x, 0);
            }
        }

        private Point CalculateGenericPanelPosition(Panel panel)
        {
            if (panel.Children.Count == 0)
                return new Point(0, 0);

            // For unknown panels, position after the last child
            var lastChild = panel.Children[panel.Children.Count - 1];
            var position = lastChild.TranslatePoint(new Point(0, 0), panel);
            return new Point(position.X, position.Y + lastChild.RenderSize.Height);
        }

        private ItemsPresenter FindItemsPresenter(ItemsControl itemsControl)
        {
            return FindVisualChild<ItemsPresenter>(itemsControl);
        }

        private Panel FindItemsPanel(ItemsPresenter itemsPresenter)
        {
            if (itemsPresenter == null) return null;

            // The ItemsPanel is typically the first child of ItemsPresenter
            if (VisualTreeHelper.GetChildrenCount(itemsPresenter) > 0)
            {
                return VisualTreeHelper.GetChild(itemsPresenter, 0) as Panel;
            }
            return null;
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T foundChild)
                    return foundChild;

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }
    }
}