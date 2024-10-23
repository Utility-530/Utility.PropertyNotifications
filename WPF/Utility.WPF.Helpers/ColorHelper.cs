using System.Drawing;
using System.Windows.Media;
using DrawingColor = System.Drawing.Color;
using MediaBrush = System.Windows.Media.Brush;
using MediaColor = System.Windows.Media.Color;

namespace Utility.WPF.Helpers
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/5641078/convert-from-color-to-brush"></a>
    /// </summary>
    public static class ColorHelper
    {
        public static MediaColor ToMediaColor(this DrawingColor color) => MediaColor.FromArgb(color.A, color.R, color.G, color.B);

        public static DrawingColor ToDrawingColor(this MediaColor color) => DrawingColor.FromArgb(color.A, color.R, color.G, color.B);

        public static MediaBrush ToMediaBrush(this DrawingColor color) => (SolidColorBrush)new BrushConverter().ConvertFrom(color.ToHexColor());

        public static MediaBrush ToMediaBrush(this MediaColor color) => new SolidColorBrush(color);

        public static MediaBrush WithOpacity(this MediaBrush brush, double opacity)
        {
            brush.Opacity = opacity;
            return brush;
        }

        public static string ToHexColor(this DrawingColor c) => "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");

        public static string ToRGBColor(this DrawingColor c) => "RGB(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
        public static MediaColor Inverse(this MediaColor c) => DrawingColor.FromArgb(c.ToDrawingColor().ToArgb() ^ 0xffffff).ToMediaColor();

        public static (DrawingColor, DrawingColor) GetColorFromRYGGradient(double percentage)
        {
            double red = (percentage > 50 ? 1 - 2 * (percentage - 50) / 100.0 : 1.0) * 255;
            double green = (percentage > 50 ? 1.0 : 2 * percentage / 100.0) * 255;
            double blue = 0.0;
            return (DrawingColor.FromArgb((int)red, (int)green, (int)blue),
                DrawingColor.FromArgb((int)green, (int)red, (int)blue));
        }
    }
}