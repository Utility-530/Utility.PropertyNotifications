using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Utility.WPF.Helpers
{
    public static class GeometryHelper
    {
        public static double Length(this Geometry geo)
        {
            return geo.Point().Skip(1).Sum(a => Math.Sqrt(Math.Pow(a.X, 2) + Math.Pow(a.Y, 2)));
        }

        public static IEnumerable<Point> Point(this Geometry geo)
        {
            PathGeometry path = geo.GetFlattenedPathGeometry();

            double length = 0.0;

            foreach (PathFigure pf in path.Figures)
            {
                Point start = pf.StartPoint;
                yield return start;

                foreach (PathSegment seg in pf.Segments)
                {
                    if (seg is PolyLineSegment pls)
                        foreach (Point point in pls.Points)
                        {
                            length += Distance(start, point);
                            start = point;
                            yield return start;
                        }
                    else if (seg is LineSegment ls)
                    {
                        var point = ls.Point;
                        length += Distance(start, point);
                        start = point;
                        yield return start;
                    }
                }
            }
        }

        public static Point StartPoint(this Geometry geo)
        {
            PathGeometry path = geo.GetFlattenedPathGeometry();

            var seg = path.Figures.First().Segments.First();
            switch (seg)
            {
                case PolyLineSegment pls:
                    return pls.Points.First();

                case LineSegment ls:
                    return ls.Point;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Point EndPoint(this Geometry geo)
        {
            PathGeometry path = geo.GetFlattenedPathGeometry();

            var seg = path.Figures.Last().Segments.Last();
            switch (seg)
            {
                case PolyLineSegment pls:
                    return pls.Points.Last();

                case LineSegment ls:
                    return ls.Point;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
    }
}