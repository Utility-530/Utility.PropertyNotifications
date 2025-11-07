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
            HoverBackgroundBrush = new System.Windows.Media.SolidColorBrush(Colors.IndianRed);
        }
    }

    public class PlusGeometryButton : GeometryButton
    {
        public PlusGeometryButton()
        {
            Data = Geometry.Parse("M0,1 H2 M1,0 V2");
            {
            }
            ;
            HoverBackgroundBrush = new System.Windows.Media.SolidColorBrush(Colors.CadetBlue);
        }
    }

    public class DuplicateGeometryButton : GeometryButton
    {
        public DuplicateGeometryButton()
        {
            Data = Geometry.Parse("M0,1 H1 M1,0 V1 M0,1 H1 M1,0 V1 M0,1 V0 M1,0 H0 M0.5,0.5 H-0.5 M-0.5,0.5 V-0.5 M0.5,0 H1 M0.5,-0.5 V0.5 M-0.5,-0.5 H0.5");
            {
            }
            ;
            HoverBackgroundBrush = new System.Windows.Media.SolidColorBrush(Colors.DarkGoldenrod);
            HoverForegroundBrush = new System.Windows.Media.SolidColorBrush(Colors.CornflowerBlue);
            PressedForegroundBrush = new System.Windows.Media.SolidColorBrush(Colors.BlanchedAlmond);
            PressedBackgroundBrush = new System.Windows.Media.SolidColorBrush(Colors.Chartreuse);
            Fill = new System.Windows.Media.SolidColorBrush(Colors.White);
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
            HoverBackgroundBrush = new System.Windows.Media.SolidColorBrush(Colors.LightCoral);
        }
    }
}