using System;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Panels
{
    /// <summary>
    /// A panel that arranges child elements to maximize the distance between
    /// themselves and the edges of the container. Items are arranged in an
    /// optimal grid layout with maximum margins on all sides.
    /// </summary>
    public class MasterSlavePanel : Panel
    {
        public static readonly DependencyProperty MasterPositionProperty =
            DependencyProperty.Register(
                nameof(MasterPosition),
                typeof(Dock),
                typeof(MasterSlavePanel),
                new FrameworkPropertyMetadata(
                    Dock.Left,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public static readonly DependencyProperty MinimumItemSpacingProperty =
            DependencyProperty.Register(
                nameof(MinimumItemSpacing),
                typeof(double),
                typeof(MasterSlavePanel),
                new FrameworkPropertyMetadata(
                    10.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Gets or sets the position of the first item (Left, Right, Top, or Bottom).
        /// </summary>
        public Dock MasterPosition
        {
            get => (Dock)GetValue(MasterPositionProperty);
            set => SetValue(MasterPositionProperty, value);
        }

        /// <summary>
        /// Gets or sets the minimum spacing between items (default: 10).
        /// </summary>
        public double MinimumItemSpacing
        {
            get => (double)GetValue(MinimumItemSpacingProperty);
            set => SetValue(MinimumItemSpacingProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            int childCount = InternalChildren.Count;
            if (childCount == 0)
                return new Size(0, 0);

            // Measure all children
            foreach (UIElement child in InternalChildren)
            {
                if (child != null)
                {
                    child.Measure(availableSize);
                }
            }

            if (childCount == 1)
            {
                // Single item: return its desired size
                return InternalChildren[0]?.DesiredSize ?? new Size(0, 0);
            }

            // Multiple items: calculate space needed for master-slave layout
            UIElement firstChild = InternalChildren[0];
            bool isHorizontal = MasterPosition == Dock.Left || MasterPosition == Dock.Right;

            double firstChildWidth = firstChild.DesiredSize.Width;
            double firstChildHeight = firstChild.DesiredSize.Height;

            // Calculate space needed for remaining items when stacked
            double remainingWidth = 0;
            double remainingHeight = 0;

            if (isHorizontal)
            {
                // First item is horizontal, remaining items stack vertically
                // Width: max width of remaining items
                // Height: sum of heights + minimum spacing
                for (int i = 1; i < InternalChildren.Count; i++)
                {
                    UIElement child = InternalChildren[i];
                    if (child != null)
                    {
                        remainingWidth = Math.Max(remainingWidth, child.DesiredSize.Width);
                        remainingHeight += child.DesiredSize.Height;
                    }
                }
                // Add minimum spacing between items (N-1 gaps for N items)
                if (childCount > 2)
                {
                    remainingHeight += MinimumItemSpacing * (childCount - 2);
                }

                // Total size
                double totalWidth = firstChildWidth + remainingWidth;
                double totalHeight = Math.Max(firstChildHeight, remainingHeight);
                return new Size(totalWidth, totalHeight);
            }
            else
            {
                // First item is vertical, remaining items stack horizontally
                // Height: max height of remaining items
                // Width: sum of widths + minimum spacing
                for (int i = 1; i < InternalChildren.Count; i++)
                {
                    UIElement child = InternalChildren[i];
                    if (child != null)
                    {
                        remainingWidth += child.DesiredSize.Width;
                        remainingHeight = Math.Max(remainingHeight, child.DesiredSize.Height);
                    }
                }
                // Add minimum spacing between items (N-1 gaps for N items)
                if (childCount > 2)
                {
                    remainingWidth += MinimumItemSpacing * (childCount - 2);
                }

                // Total size
                double totalWidth = Math.Max(firstChildWidth, remainingWidth);
                double totalHeight = firstChildHeight + remainingHeight;
                return new Size(totalWidth, totalHeight);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int childCount = InternalChildren.Count;
            if (childCount == 0)
                return finalSize;

            if (childCount == 1)
            {
                // Single item: center it with maximum distance from all edges
                UIElement child = InternalChildren[0];
                if (child != null)
                {
                    double x = (finalSize.Width - child.DesiredSize.Width) / 2;
                    double y = (finalSize.Height - child.DesiredSize.Height) / 2;
                    child.Arrange(new Rect(x, y, child.DesiredSize.Width, child.DesiredSize.Height));
                }
                return finalSize;
            }

            // Multiple items: first item at edge, others in optimal grid
            UIElement firstChild = InternalChildren[0];
            bool isHorizontal = MasterPosition == Dock.Left || MasterPosition == Dock.Right;

            // Get first child size
            double firstChildWidth = firstChild.DesiredSize.Width;
            double firstChildHeight = firstChild.DesiredSize.Height;

            // Calculate available space for remaining items
            Size remainingSpace;
            if (isHorizontal)
            {
                remainingSpace = new Size(
                    finalSize.Width - firstChildWidth,
                    finalSize.Height
                );
            }
            else
            {
                remainingSpace = new Size(
                    finalSize.Width,
                    finalSize.Height - firstChildHeight
                );
            }

            // Arrange remaining items and get their bounding box
            var remainingBounds = ArrangeRemainingItemsWithMaxDistance(remainingSpace, isHorizontal);

            // Now position the first item to maximize distance from edges and other items
            ArrangeFirstItemWithMaxDistance(finalSize, firstChild, remainingBounds, isHorizontal);

            return finalSize;
        }

        private Rect ArrangeRemainingItemsWithMaxDistance(Size availableSpace, bool isHorizontal)
        {
            int remainingCount = InternalChildren.Count - 1;
            if (remainingCount == 0)
                return Rect.Empty;

            // Arrange remaining items perpendicular to first item
            // Maximize distance between CENTERS while keeping items within bounds

            // Calculate maximum item sizes for remaining items
            double maxItemWidth = 0;
            double maxItemHeight = 0;
            for (int i = 1; i < InternalChildren.Count; i++)
            {
                UIElement child = InternalChildren[i];
                if (child != null)
                {
                    maxItemWidth = Math.Max(maxItemWidth, child.DesiredSize.Width);
                    maxItemHeight = Math.Max(maxItemHeight, child.DesiredSize.Height);
                }
            }

            // Calculate offset based on first item position
            double offsetX = 0;
            double offsetY = 0;

            if (MasterPosition == Dock.Right)
            {
                offsetX = 0; // Remaining items on left
            }
            else if (MasterPosition == Dock.Left)
            {
                offsetX = InternalChildren[0].DesiredSize.Width; // Remaining items on right
            }
            else if (MasterPosition == Dock.Bottom)
            {
                offsetY = 0; // Remaining items on top
            }
            else if (MasterPosition == Dock.Top)
            {
                offsetY = InternalChildren[0].DesiredSize.Height; // Remaining items on bottom
            }

            // Track the bounding box of all remaining items
            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            if (isHorizontal)
            {
                // First item is horizontal (Left/Right), arrange others VERTICALLY
                // For N items, we have N+1 equal gaps:
                // [edge] --gap-- [center1] --gap-- [center2] --gap-- ... --gap-- [centerN] --gap-- [edge]
                // Each gap = availableHeight / (N+1)

                double idealGap = availableSpace.Height / (remainingCount + 1);

                // The minimum gap needed to fit items without overlap
                double minGapForBounds = maxItemHeight / 2;

                // Choose the appropriate gap
                double gap;
                if (idealGap >= minGapForBounds)
                {
                    // Plenty of space, use ideal spacing
                    gap = idealGap;
                }
                else
                {
                    // Not enough space for ideal layout
                    // Calculate total height needed for items
                    double totalItemHeight = 0;
                    for (int i = 1; i < InternalChildren.Count; i++)
                    {
                        UIElement child = InternalChildren[i];
                        if (child != null)
                        {
                            totalItemHeight += child.DesiredSize.Height;
                        }
                    }

                    // Calculate remaining space for gaps
                    double remainingGapSpace = availableSpace.Height - totalItemHeight;

                    // Distribute remaining space as gaps, but don't go below MinimumItemSpacing
                    // We have (N-1) gaps between items + 2 edge gaps = N+1 total gaps
                    double calculatedGap = Math.Max(0, remainingGapSpace / (remainingCount + 1));

                    // Use at least MinimumItemSpacing between items, but edge gaps can be smaller
                    gap = calculatedGap;
                }

                for (int i = 1; i < InternalChildren.Count; i++)
                {
                    UIElement child = InternalChildren[i];
                    if (child != null)
                    {
                        int itemIndex = i - 1; // 0-based index in remaining items

                        // Center Y position: gap * (itemIndex + 1)
                        double centerY = offsetY + gap * (itemIndex + 1);

                        // Calculate actual position (top-left corner)
                        double x = offsetX + (availableSpace.Width - child.DesiredSize.Width) / 2;
                        double y = centerY - child.DesiredSize.Height / 2;

                        // Clamp to bounds
                        y = Math.Max(0, Math.Min(y, availableSpace.Height - child.DesiredSize.Height));

                        child.Arrange(new Rect(x, y, child.DesiredSize.Width, child.DesiredSize.Height));

                        minX = Math.Min(minX, x);
                        minY = Math.Min(minY, y);
                        maxX = Math.Max(maxX, x + child.DesiredSize.Width);
                        maxY = Math.Max(maxY, y + child.DesiredSize.Height);
                    }
                }
            }
            else
            {
                // First item is vertical (Top/Bottom), arrange others HORIZONTALLY
                // For N items, we have N+1 equal gaps:
                // [edge] --gap-- [center1] --gap-- [center2] --gap-- ... --gap-- [centerN] --gap-- [edge]
                // Each gap = availableWidth / (N+1)

                double idealGap = availableSpace.Width / (remainingCount + 1);

                // The minimum gap needed to fit items without overlap
                double minGapForBounds = maxItemWidth / 2;

                // Choose the appropriate gap
                double gap;
                if (idealGap >= minGapForBounds)
                {
                    // Plenty of space, use ideal spacing
                    gap = idealGap;
                }
                else
                {
                    // Not enough space for ideal layout
                    // Calculate total width needed for items
                    double totalItemWidth = 0;
                    for (int i = 1; i < InternalChildren.Count; i++)
                    {
                        UIElement child = InternalChildren[i];
                        if (child != null)
                        {
                            totalItemWidth += child.DesiredSize.Width;
                        }
                    }

                    // Calculate remaining space for gaps
                    double remainingGapSpace = availableSpace.Width - totalItemWidth;

                    // Distribute remaining space as gaps
                    double calculatedGap = Math.Max(0, remainingGapSpace / (remainingCount + 1));

                    gap = calculatedGap;
                }

                for (int i = 1; i < InternalChildren.Count; i++)
                {
                    UIElement child = InternalChildren[i];
                    if (child != null)
                    {
                        int itemIndex = i - 1; // 0-based index in remaining items

                        // Center X position: gap * (itemIndex + 1)
                        double centerX = offsetX + gap * (itemIndex + 1);

                        // Calculate actual position (top-left corner)
                        double x = centerX - child.DesiredSize.Width / 2;
                        double y = offsetY + (availableSpace.Height - child.DesiredSize.Height) / 2;

                        // Clamp to bounds
                        x = Math.Max(0, Math.Min(x, availableSpace.Width - child.DesiredSize.Width));

                        child.Arrange(new Rect(x, y, child.DesiredSize.Width, child.DesiredSize.Height));

                        minX = Math.Min(minX, x);
                        minY = Math.Min(minY, y);
                        maxX = Math.Max(maxX, x + child.DesiredSize.Width);
                        maxY = Math.Max(maxY, y + child.DesiredSize.Height);
                    }
                }
            }

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        private void ArrangeFirstItemWithMaxDistance(Size finalSize, UIElement firstChild, Rect remainingBounds, bool isHorizontal)
        {
            double x, y;

            if (isHorizontal)
            {
                // Vertical centering
                y = (finalSize.Height - firstChild.DesiredSize.Height) / 2;

                if (MasterPosition == Dock.Left)
                {
                    // First item on left, remaining items on right
                    // We want equal gaps: edge-to-center, center-to-remaining-items
                    // Let gap = g
                    // First item center at: g
                    // Distance from first item to remaining items should also be g
                    // So: g + firstChildWidth/2 + g = remainingBounds.Left
                    // Therefore: g = (remainingBounds.Left - firstChildWidth/2) / 2

                    double gap = (remainingBounds.Left - firstChild.DesiredSize.Width / 2) / 2;
                    gap = Math.Max(gap, firstChild.DesiredSize.Width / 2); // Ensure item fits

                    double firstItemCenterX = gap;
                    x = firstItemCenterX - firstChild.DesiredSize.Width / 2;
                }
                else // Dock.Right
                {
                    // First item on right, remaining items on left
                    // Gap from remaining items to first item = gap from first item to edge
                    // remainingBounds.Right + g = first item center
                    // first item center + g = finalSize.Width
                    // So: first item center = (remainingBounds.Right + finalSize.Width) / 2

                    double gap = (finalSize.Width - remainingBounds.Right - firstChild.DesiredSize.Width / 2) / 2;
                    gap = Math.Max(gap, firstChild.DesiredSize.Width / 2);

                    double firstItemCenterX = finalSize.Width - gap;
                    x = firstItemCenterX - firstChild.DesiredSize.Width / 2;
                }
            }
            else // Vertical
            {
                // Horizontal centering
                x = (finalSize.Width - firstChild.DesiredSize.Width) / 2;

                if (MasterPosition == Dock.Top)
                {
                    // First item on top, remaining items below
                    double gap = (remainingBounds.Top - firstChild.DesiredSize.Height / 2) / 2;
                    gap = Math.Max(gap, firstChild.DesiredSize.Height / 2);

                    double firstItemCenterY = gap;
                    y = firstItemCenterY - firstChild.DesiredSize.Height / 2;
                }
                else // Dock.Bottom
                {
                    // First item on bottom, remaining items above
                    double gap = (finalSize.Height - remainingBounds.Bottom - firstChild.DesiredSize.Height / 2) / 2;
                    gap = Math.Max(gap, firstChild.DesiredSize.Height / 2);

                    double firstItemCenterY = finalSize.Height - gap;
                    y = firstItemCenterY - firstChild.DesiredSize.Height / 2;
                }
            }

            firstChild.Arrange(new Rect(x, y, firstChild.DesiredSize.Width, firstChild.DesiredSize.Height));
        }
    }
}