using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls
{
    public class SpacedTextBlock : Control
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text",
            typeof(string),
            typeof(SpacedTextBlock));

        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (Text != null)
            {
                var widthPerChar = 1 * ActualWidth / Text.Length;
                var currentPosition = 0.33 * ActualWidth / Text.Length;

                foreach (var ch in Text)
                {
                    drawingContext.DrawText(new FormattedText(ch.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 12, Foreground), new Point(currentPosition, 0));
                    currentPosition += widthPerChar;
                }
            }
        }
    }
}
