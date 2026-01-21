using System;
using System.Windows;
using System.Windows.Controls;

namespace CustomPanels
{
    /// <summary>
    /// A panel that places the first child at a specified edge (Left, Right, Top, Bottom)
    /// and spaces the remaining children as far apart as possible in the remaining space.
    /// </summary>
    public class SpacingPanel : Panel
    {
        public static readonly DependencyProperty FirstItemPositionProperty =
            DependencyProperty.Register(
                nameof(FirstItemPosition),
                typeof(Dock),
                typeof(SpacingPanel),
                new FrameworkPropertyMetadata(
                    Dock.Left,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Gets or sets the position of the first item (Left, Right, Top, or Bottom).
        /// </summary>
        public Dock FirstItemPosition
        {
            get => (Dock)GetValue(FirstItemPositionProperty);
            set => SetValue(FirstItemPositionProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size totalSize = new Size(0, 0);

            foreach (UIElement child in InternalChildren)
            {
                if (child != null)
                {
                    child.Measure(availableSize);
                    totalSize.Width = Math.Max(totalSize.Width, child.DesiredSize.Width);
                    totalSize.Height = Math.Max(totalSize.Height, child.DesiredSize.Height);
                }
            }

            return totalSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (InternalChildren.Count == 0)
                return finalSize;

            bool isHorizontal = FirstItemPosition == Dock.Left || FirstItemPosition == Dock.Right;

            // Arrange the first child at the specified edge
            UIElement firstChild = InternalChildren[0];
            if (firstChild != null)
            {
                Rect firstRect = GetFirstChildRect(firstChild, finalSize);
                firstChild.Arrange(firstRect);
            }

            // Arrange remaining children with maximum spacing
            if (InternalChildren.Count > 1)
            {
                ArrangeRemainingChildren(finalSize, isHorizontal);
            }

            return finalSize;
        }

        private Rect GetFirstChildRect(UIElement firstChild, Size finalSize)
        {
            Size childSize = firstChild.DesiredSize;

            switch (FirstItemPosition)
            {
                case Dock.Left:
                    return new Rect(0, 0, childSize.Width, finalSize.Height);

                case Dock.Right:
                    return new Rect(finalSize.Width - childSize.Width, 0, childSize.Width, finalSize.Height);

                case Dock.Top:
                    return new Rect(0, 0, finalSize.Width, childSize.Height);

                case Dock.Bottom:
                    return new Rect(0, finalSize.Height - childSize.Height, finalSize.Width, childSize.Height);

                default:
                    return new Rect(0, 0, childSize.Width, childSize.Height);
            }
        }

        private void ArrangeRemainingChildren(Size finalSize, bool isHorizontal)
        {
            int remainingCount = InternalChildren.Count - 1;

            if (isHorizontal)
            {
                ArrangeHorizontally(finalSize, remainingCount);
            }
            else
            {
                ArrangeVertically(finalSize, remainingCount);
            }
        }

        private void ArrangeHorizontally(Size finalSize, int remainingCount)
        {
            UIElement firstChild = InternalChildren[0];
            double firstChildWidth = firstChild.DesiredSize.Width;
            double availableWidth = finalSize.Width - firstChildWidth;

            // Calculate total width needed for remaining children
            double totalChildrenWidth = 0;
            for (int i = 1; i < InternalChildren.Count; i++)
            {
                totalChildrenWidth += InternalChildren[i].DesiredSize.Width;
            }

            // Calculate spacing between children
            double spacing = 0;
            if (remainingCount > 1 && availableWidth > totalChildrenWidth)
            {
                spacing = (availableWidth - totalChildrenWidth) / (remainingCount - 1);
            }

            // Start position after the first child (or at the beginning if first child is on right)
            double currentX = FirstItemPosition == Dock.Left ? firstChildWidth : 0;

            for (int i = 1; i < InternalChildren.Count; i++)
            {
                UIElement child = InternalChildren[i];
                Size childSize = child.DesiredSize;

                double y = (finalSize.Height - childSize.Height) / 2; // Center vertically
                child.Arrange(new Rect(currentX, y, childSize.Width, childSize.Height));

                currentX += childSize.Width + spacing;
            }
        }

        private void ArrangeVertically(Size finalSize, int remainingCount)
        {
            UIElement firstChild = InternalChildren[0];
            double firstChildHeight = firstChild.DesiredSize.Height;
            double availableHeight = finalSize.Height - firstChildHeight;

            // Calculate total height needed for remaining children
            double totalChildrenHeight = 0;
            for (int i = 1; i < InternalChildren.Count; i++)
            {
                totalChildrenHeight += InternalChildren[i].DesiredSize.Height;
            }

            // Calculate spacing between children
            double spacing = 0;
            if (remainingCount > 1 && availableHeight > totalChildrenHeight)
            {
                spacing = (availableHeight - totalChildrenHeight) / (remainingCount - 1);
            }

            // Start position after the first child (or at the beginning if first child is on bottom)
            double currentY = FirstItemPosition == Dock.Top ? firstChildHeight : 0;

            for (int i = 1; i < InternalChildren.Count; i++)
            {
                UIElement child = InternalChildren[i];
                Size childSize = child.DesiredSize;

                double x = (finalSize.Width - childSize.Width) / 2; // Center horizontally
                child.Arrange(new Rect(x, currentY, childSize.Width, childSize.Height));

                currentY += childSize.Height + spacing;
            }
        }
    }
}