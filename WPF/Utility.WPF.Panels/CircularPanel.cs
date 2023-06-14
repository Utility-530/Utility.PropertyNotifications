using System;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Panels
{
    /// <summary>
    /// <a href="https://wpf.2000things.com/tag/panel/"></a>
    /// </summary>
    public class CircularPanel : Panel
    {
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double?),
            typeof(CircularPanel),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange));

        public double? Radius
        {
            get { return (double?)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return base.MeasureOverride(availableSize);
        }

        // Arrange stuff in a circle
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count > 0)
            {
                // Center & radius of panel
                Point center = new Point(finalSize.Width / 2, finalSize.Height / 2);
                double radius = Radius ?? GetRadius();

                // # radians between children
                double angleIncrRadians = 2.0 * Math.PI / Children.Count;

                double angleInRadians = 0.0;

                foreach (UIElement child in Children)
                {
                    Point childPosition = new Point(
                        radius * Math.Cos(angleInRadians) + center.X,
                        radius * Math.Sin(angleInRadians) + center.Y);

                    child.Arrange(new Rect(childPosition, child.DesiredSize));

                    angleInRadians += angleIncrRadians;
                }
            }

            return finalSize;

            double GetRadius()
            {
                double radius = Math.Min(finalSize.Width, finalSize.Height) / 2.0;
                radius *= 0.8;   // To avoid hitting edges
                return radius;
            }
        }
    }
}