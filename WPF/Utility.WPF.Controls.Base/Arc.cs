using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Utility.WPF.Controls
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/23046565/wpf-radial-progressbar-meter-i-e-battery-meter"></a>
    /// </summary>
    public class Arc : Shape
    {
        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register("StartAngle", typeof(double), typeof(Arc), new UIPropertyMetadata(0.0, new PropertyChangedCallback(UpdateArc)));
        public static readonly DependencyProperty EndAngleProperty = DependencyProperty.Register("EndAngle", typeof(double), typeof(Arc), new UIPropertyMetadata(90.0, new PropertyChangedCallback(UpdateArc)));
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register("Direction", typeof(SweepDirection), typeof(Arc), new UIPropertyMetadata(SweepDirection.Clockwise));

        public static readonly DependencyProperty OriginRotationDegreesProperty = DependencyProperty.Register("OriginRotationDegrees", typeof(double), typeof(Arc),
                new UIPropertyMetadata(270.0, new PropertyChangedCallback(UpdateArc)));

        #region properties

        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        public double EndAngle
        {
            get { return (double)GetValue(EndAngleProperty); }
            set { SetValue(EndAngleProperty, value); }
        }

        //This controls whether or not the progress bar goes clockwise or counterclockwise
        public SweepDirection Direction
        {
            get { return (SweepDirection)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        //rotate the start/endpoint of the arc a certain number of degree in the direction
        //ie. if you wanted it to be at 12:00 that would be 270 Clockwise or 90 counterclockwise
        public double OriginRotationDegrees
        {
            get { return (double)GetValue(OriginRotationDegreesProperty); }
            set { SetValue(OriginRotationDegreesProperty, value); }
        }

        #endregion properties

        protected static void UpdateArc(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Arc arc = d as Arc;
            arc.InvalidateVisual();
        }

        protected override Geometry DefiningGeometry => GetArcGeometry();

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawGeometry(null, new Pen(Stroke, StrokeThickness), GetArcGeometry());
        }

        private Geometry GetArcGeometry()
        {
            Point startPoint = PointAtAngle(Math.Min(StartAngle, EndAngle), Direction);
            Point endPoint = PointAtAngle(Math.Max(StartAngle, EndAngle), Direction);

            Size arcSize = new Size(Math.Max(0, (RenderSize.Width - StrokeThickness) / 2),
                Math.Max(0, (RenderSize.Height - StrokeThickness) / 2));
            bool isLargeArc = Math.Abs(EndAngle - StartAngle) > 180;

            StreamGeometry geom = new StreamGeometry();// BuildRegularPolygon(new Point(0, 9), 2, 4, 30);
            using (StreamGeometryContext context = geom.Open())
            {
                context.BeginFigure(startPoint, false, false);
                context.ArcTo(endPoint, arcSize, 0, isLargeArc, Direction, true, false);

                var c = new EllipseGeometry(startPoint, 0, 0);

                var c2 = new EllipseGeometry(endPoint, 0, 0);

                GeometryGroup combined = new GeometryGroup();
                combined.Children.Add(c);
                combined.Children.Add(c2);
                combined.Children.Add(geom);

                combined.Transform = new TranslateTransform(StrokeThickness / 2, StrokeThickness / 2);

                return combined;
            }
        }

        private StreamGeometry BuildRegularPolygon(Point c, double r, int numSides, double offsetDegree)
        {
            // c is the center, r is the radius,
            // numSides the number of sides, offsetDegree the offset in Degrees.
            // Do not add the last point.

            StreamGeometry geometry = new StreamGeometry();

            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(new Point(), true /* is filled */, true /* is closed */);

                double step = 2 * Math.PI / Math.Max(numSides, 3);
                Point cur = c;

                double a = Math.PI * offsetDegree / 180.0;
                for (int i = 0; i < numSides; i++, a += step)
                {
                    cur.X = c.X + r * Math.Cos(a);
                    cur.Y = c.Y + r * Math.Sin(a);
                    ctx.LineTo(cur, true /* is stroked */, false /* is smooth join */);
                }
            }

            return geometry;
        }

        private Point PointAtAngle(double angle, SweepDirection sweep)
        {
            double translatedAngle = angle + OriginRotationDegrees;
            double radAngle = translatedAngle * (Math.PI / 180);
            double xr = (RenderSize.Width - StrokeThickness) / 2;
            double yr = (RenderSize.Height - StrokeThickness) / 2;

            double x = xr + xr * Math.Cos(radAngle);
            double y = yr * Math.Sin(radAngle);

            if (sweep == SweepDirection.Counterclockwise)
            {
                y = yr - y;
            }
            else
            {
                y = yr + y;
            }

            return new Point(x, y);
        }
    }

    public class ProgressToAngleConverter : System.Windows.Data.IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double progress = (double)values[0];
            System.Windows.Controls.ProgressBar bar = values[1] as System.Windows.Controls.ProgressBar;

            return 359.999 * (progress / (bar.Maximum - bar.Minimum));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}