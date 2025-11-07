using System.Windows;
using Utility.Enums;

namespace Utility.WPF.Trees.Connectors
{
    internal class StraightPathFinder
    {
        public static List<Point> GetConnectionLine(Point source, Point sinkPoint, Position2D preferredOrientation)
        {
            List<Point> linePoints = [source, sinkPoint];
            return linePoints;
        }
    }
}