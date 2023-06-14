using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Panels
{
    /// <summary>
    /// UniformGrid is used to arrange children in a grid with all equal cell sizes.
    /// </summary>
    public class UniformRow : Panel
    {
        private int _columns;

        protected override Size MeasureOverride(Size constraint)
        {
            _columns = InternalChildren.Count > 1 ? InternalChildren.Count - 1 : InternalChildren.Count;

            Size childConstraint = new Size(constraint.Width / _columns, constraint.Height);
            double maxChildDesiredWidth = 0.0;
            double maxChildDesiredHeight = 0.0;

            //  Measure each child, keeping track of maximum desired width and height.
            for (int i = 0, count = InternalChildren.Count; i < count; ++i)
            {
                UIElement child = InternalChildren[i];

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

            return new Size(maxChildDesiredWidth * _columns, maxChildDesiredHeight);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Rect childBounds = new Rect(0, 0, arrangeSize.Width / _columns, arrangeSize.Height / 1);
            double xStep = childBounds.Width;
            double xBound = arrangeSize.Width - 1.0;

            // Arrange and Position each child to the same cell size
            foreach (UIElement child in InternalChildren)
            {
                child.Arrange(new Rect(childBounds.X - child.DesiredSize.Width / 2d, childBounds.Y, childBounds.Width, childBounds.Height));

                // only advance to the next grid cell if the child was not collapsed
                if (child.Visibility != Visibility.Collapsed)
                {
                    childBounds.X += xStep;
                }
            }

            return arrangeSize;
        }
    }
}