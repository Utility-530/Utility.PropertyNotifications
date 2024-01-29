using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Documents;

namespace Utility.WPF.Demo.SandBox
{
    public class NotificationAdorner : Adorner
    {
        public NotificationAdorner(UIElement adornedElement)
            : base(adornedElement)
        {

        }

  

        protected override void OnRender(DrawingContext drawingContext)
        {
            var adornedElementRect = new Rect(AdornedElement.DesiredSize);
            var typeFace = new Typeface(new FontFamily("Courier New"), FontStyles.Normal, FontWeights.ExtraBold,
                                        FontStretches.Condensed);

            var text = new FormattedText(
                5.ToString(),
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.RightToLeft,
                typeFace,
                16, Brushes.White
                );

            var pointOfOrigin = new Point
            {
                X = adornedElementRect.BottomRight.X,
                Y = adornedElementRect.BottomRight.Y - text.Height * 0.7
            };

            var elipseCenter = new Point
            {
                X = pointOfOrigin.X - text.Width / 2,
                Y = pointOfOrigin.Y + text.Height / 2
            };

            var elipseBrush = new SolidColorBrush
            {
                Color = Colors.DarkRed,
                Opacity = 0.8
            };

            drawingContext.DrawEllipse(
                elipseBrush,
                new Pen(Brushes.White, 2),
                elipseCenter,
                text.Width * 0.9,
                text.Height * 0.5
                );

            drawingContext.DrawText(text, pointOfOrigin);
        }
    }
}
