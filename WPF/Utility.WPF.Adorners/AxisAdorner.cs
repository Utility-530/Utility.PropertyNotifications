using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Utility.WPF.Adorners
{
    public class HorizontalAxis : FrameworkElement
    {
        private const double startPoint = 0.0;
        private const double endPoint = 600.0;

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.OnRender(startPoint, endPoint, ActualHeight / 2, Orientation.Horizontal);
        }
    }

    public class HorizontalAxisAdorner : Adorner
    {
        private const double startPoint = 0.0;
        private const double endPoint = 600.0;

        public HorizontalAxisAdorner(UIElement adornedElement) : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.OnRender(startPoint, endPoint, ActualHeight / 2, Orientation.Horizontal);
        }
    }

    public class VerticalAxis : FrameworkElement
    {
        protected override void OnRender(DrawingContext drawingContext)
        {
            double startPoint = ActualHeight / 2;
            double endPoint = ActualHeight / 2 - 300;
            base.OnRender(drawingContext);

            drawingContext.OnRender(startPoint, endPoint, 0, Orientation.Vertical);
        }
    }

    public class VerticalAxisAdorner : Adorner
    {
        public VerticalAxisAdorner(UIElement adornedElement) : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var startPoint = ActualHeight / 2;
            var endPoint = ActualHeight / 2 - 300;
            drawingContext.OnRender(startPoint, endPoint, 0, Orientation.Vertical);
        }
    }

    public static class AxisHelper
    {
        private static Pen mainPen = new Pen(Brushes.Black, 1.0);

        public static void OnRender(this DrawingContext drawingContext, double startValue, double endValue, double position, Orientation orientation)
        {
            double length = Math.Abs(endValue - startValue);

            //Draw horizontal line from startPoint to endPoint
            if (orientation == Orientation.Horizontal)
                drawingContext.DrawLine(mainPen, new Point(startValue, position),
                                                     new Point(endValue, position));
            else
                drawingContext.DrawLine(mainPen, new Point(position, startValue),
                                     new Point(position, endValue));

            var typeFace = new Typeface(new FontFamily("Segoe UI"),
                             FontStyles.Normal,
                             FontWeights.Normal,
                             FontStretches.Normal);

            var factor = endValue > startValue ? 1 : -1;
            // Draw ticks and text
            for (double i = 0; i <= length; i += 50)
            {
                var value = startValue + i * factor;
                // Draw vertical ticks on the horizontal line drawn above.
                // They are spaced apart by 50 pixels.
                if (orientation == Orientation.Horizontal)
                    drawingContext.DrawLine(mainPen, new Point(value, position), new Point(value, position + 10));
                else
                    drawingContext.DrawLine(mainPen, new Point(position, value), new Point(position - 10, value));

                // Draw text below every tick
                FormattedText ft = new FormattedText(
                       i.ToString(CultureInfo.CurrentCulture),
                                    CultureInfo.CurrentCulture,
                                    FlowDirection.RightToLeft,
                                    typeFace,
                                    12,
                                    Brushes.Black,
                                    null,
                                    TextFormattingMode.Display);

                if (orientation == Orientation.Horizontal)
                    drawingContext.DrawText(ft, new Point(value, position + 20));
                else
                    drawingContext.DrawText(ft, new Point(position - 12, value - ft.Height / 2d));
            }
        }
    }
}