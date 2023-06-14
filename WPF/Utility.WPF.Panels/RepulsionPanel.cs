using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Panels
{
    /// <summary>
    /// Maximixes the distance between elements in a row
    /// </summary>
    public class RepulsionPanel : Panel
    {
        public static readonly DependencyProperty OrientationProperty =
       DependencyProperty.Register("Orientation", typeof(Orientation), typeof(RepulsionPanel), new PropertyMetadata(Orientation.Horizontal));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            //  Measure each child, keeping track of maximum desired width and height.
            var maxHeight = 0d;
            var maxWidth = 0d;
            for (int i = 0, count = InternalChildren.Count; i < count; ++i)
            {
                UIElement child = InternalChildren[i];
                child.Measure(constraint);
                Size childDesiredSize = child.DesiredSize;
                maxHeight = Math.Max(childDesiredSize.Height, maxHeight);
                maxWidth = Math.Max(childDesiredSize.Width, maxWidth);
            }

            return new Size(
                double.IsInfinity(constraint.Width) ? maxWidth : constraint.Width,
                double.IsInfinity(constraint.Height) ? maxHeight : constraint.Height);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (Orientation == Orientation.Horizontal)
            {
                var width = arrangeSize.Width;

                var space = InternalChildren.Cast<UIElement>().Sum(e => e.DesiredSize.Width);
                var gap = (width - space) / (InternalChildren.Count - 1);
                int i = 0;

                double sumWidth = 0;
                // Arrange and Position each child to the same cell size
                foreach (UIElement child in InternalChildren)
                {
                    child.Arrange(new Rect(sumWidth + gap * i, (arrangeSize.Height - child.DesiredSize.Height) / 2d, child.DesiredSize.Width, child.DesiredSize.Height));
                    sumWidth += child.DesiredSize.Width;
                    i++;
                }
            }
            else
            {
                var height = arrangeSize.Height;

                var space = InternalChildren.Cast<UIElement>().Sum(e => e.DesiredSize.Height);
                var gap = (height - space) / (InternalChildren.Count - 1);
                int i = 0;

                double sumHeight = 0;
                // Arrange and Position each child to the same cell size
                foreach (UIElement child in InternalChildren)
                {
                    child.Arrange(new Rect((arrangeSize.Width - child.DesiredSize.Width) / 2d, sumHeight + gap * i, child.DesiredSize.Width, child.DesiredSize.Height));
                    sumHeight += child.DesiredSize.Height;
                    i++;
                }
            }
            return arrangeSize;
        }
    }
}