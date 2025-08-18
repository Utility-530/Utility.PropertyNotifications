using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Panels
{
    /// <summary>
    /// UniformRow is used to arrange children in a single row with all equal cell sizes.
    /// </summary>
    public class UniformRow : Panel
    {
        protected override Size MeasureOverride(Size constraint)
        {
            return MeasureOverride(constraint, InternalChildren);
        }

        public static Size MeasureOverride(Size constraint, UIElementCollection internalChildren)
        {
            if (internalChildren.Count == 0)
                return new Size(0, 0);

            int visibleChildren = GetVisibleChildrenCount(internalChildren);
            if (visibleChildren == 0)
                return new Size(0, 0);

            Size childConstraint = new Size(constraint.Width / visibleChildren, constraint.Height);
            double maxChildDesiredWidth = 0.0;
            double maxChildDesiredHeight = 0.0;

            // Measure each child, keeping track of maximum desired width and height.
            for (int i = 0; i < internalChildren.Count; i++)
            {
                UIElement child = internalChildren[i];

                if (child.Visibility == Visibility.Collapsed)
                    continue;

                // Measure the child.
                child.Measure(childConstraint);
                Size childDesiredSize = child.DesiredSize;

                if (maxChildDesiredWidth < childDesiredSize.Width)
                {
                    maxChildDesiredWidth = childDesiredSize.Width;
                }

                if (maxChildDesiredHeight < childDesiredSize.Height)
                {
                    maxChildDesiredHeight = childDesiredSize.Height;
                }
            }

            return new Size(maxChildDesiredWidth * visibleChildren, maxChildDesiredHeight);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            return ArrangeOverride(InternalChildren, arrangeSize);
        }

        public static Size ArrangeOverride(UIElementCollection internalChildren, Size arrangeSize)
        {
            if (internalChildren.Count == 0)
                return arrangeSize;

            int visibleChildren = GetVisibleChildrenCount(internalChildren);
            if (visibleChildren == 0)
                return arrangeSize;

            double cellWidth = arrangeSize.Width / visibleChildren;
            double currentX = 0;

            // Arrange and Position each child to the same cell size
            foreach (UIElement child in internalChildren)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    // Still need to arrange collapsed children (required by WPF)
                    child.Arrange(new Rect(0, 0, 0, 0));
                    continue;
                }

                // Arrange child to fill the entire cell
                Rect childBounds = new Rect(currentX, 0, cellWidth, arrangeSize.Height);
                child.Arrange(childBounds);

                currentX += cellWidth;
            }

            return arrangeSize;
        }

        private static int GetVisibleChildrenCount(UIElementCollection internalChildren)
        {
            int count = 0;
            foreach (UIElement child in internalChildren)
            {
                if (child.Visibility != Visibility.Collapsed)
                    count++;
            }
            return count;
        }
    }
}