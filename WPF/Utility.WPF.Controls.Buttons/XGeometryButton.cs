using System.Windows.Media;

namespace Utility.WPF.Controls.Buttons
{
    public class XGeometryButton : GeometryButton
    {
        public XGeometryButton()
        {
            Data = new PathGeometry
            {
                Figures =
                new PathFigureCollection {
                    new PathFigure { StartPoint = new System.Windows.Point(0, 0), Segments = new PathSegmentCollection
                    {
                        new LineSegment { Point = new System.Windows.Point(1, 1) }
                    }
                    }, new PathFigure { StartPoint = new System.Windows.Point(0,1), Segments = new PathSegmentCollection
                    {
                        new LineSegment { Point = new System.Windows.Point(1, 0) }
                    } }
                }
            };
            HoverBackground = new System.Windows.Media.SolidColorBrush(Colors.IndianRed);
        }
    }

    public class PlusGeometryButton : GeometryButton
    {
        public PlusGeometryButton()
        {
            Data = Geometry.Parse("M0,1 H2 M1,0 V2");
            {

            };
            HoverBackground = new System.Windows.Media.SolidColorBrush(Colors.CadetBlue);
        }
    }

    public class MinusGeometryButton : GeometryButton
    {
        public MinusGeometryButton()
        {
            Data = new PathGeometry
            {
                Figures =
                    new PathFigureCollection {
                    new PathFigure { StartPoint = new System.Windows.Point(0, 0), 
                        Segments = new PathSegmentCollection
                    {
                        new LineSegment { Point = new System.Windows.Point(1, 0) }
                    }
                    }
                }
            };
            HoverBackground = new System.Windows.Media.SolidColorBrush(Colors.LightCoral);
        }
    }
}

