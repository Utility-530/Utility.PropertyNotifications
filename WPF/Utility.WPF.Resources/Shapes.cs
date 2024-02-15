using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Utility.WPF.Resources
{
    public class Shapes
    {
        public static GeometryGroup Ellipse
        {
            get
            {
                double CapDiameter = 2;
                Point ellipseCenter1 = new(0, 0);
                Point ellipseCenter2 = new(16, 0);
                Point ellipseCenter3 = new(32, 0);
                var ellipse1 = new EllipseGeometry(ellipseCenter1, CapDiameter, CapDiameter);
                var ellipse2 = new EllipseGeometry(ellipseCenter2, CapDiameter, CapDiameter);
                var ellipse3 = new EllipseGeometry(ellipseCenter3, CapDiameter, CapDiameter);
                GeometryGroup combined = new GeometryGroup();
                combined.Children.Add(ellipse1);
                combined.Children.Add(ellipse2);
                combined.Children.Add(ellipse3);

                //string sData = "M19.375 36.7818V100.625C19.375 102.834 21.1659 104.625 23.375 104.625H87.2181C90.7818 104.625 92.5664 100.316 90.0466 97.7966L26.2034 33.9534C23.6836 31.4336 19.375 33.2182 19.375 36.7818Z";
                //Path path = new Path() { Stroke = new SolidColorBrush(Colors.Black), Fill = new SolidColorBrush(Colors.Black), StrokeThickness = 2, Stretch = Stretch.Uniform };
                //string sData = "M 250,40 L200,20 L200,60 Z";
                //var converter = TypeDescriptor.GetConverter(typeof(Geometry));
                //path.Data = (Geometry)converter.ConvertFrom(sData);
                //path.Data = combined;
                //return path;
                return combined;
            }
        }
    }
}
