using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Utility.WPF.Demo.Panels.Infrastructure
{
    public interface ISizer
    {
        Point Add(Size size);

        Point Add(Vector size);

        Size GetTotalSize();
    }

    public class Sizer : ISizer
    {
        private readonly IOrganiser<Vector> organiser;
        private readonly int xFactor;
        private readonly int yFactor;

        public Sizer(IOrganiser<Vector> organiser, int xFactor, int yFactor)
        {
            this.organiser = organiser;
            this.xFactor = xFactor;
            this.yFactor = yFactor;
        }

        public Point Add(Vector size)
        {
            var (x, y) = organiser.Add(size);
            var xValue = x.GroupBy(a => a.Item1).Select(a => a.Max(c => c.Item2.X * xFactor)).Sum();
            var yValue = y.GroupBy(a => a.Item1).Select(a => a.Max(c => c.Item2.Y * yFactor)).Sum();
            return new Point(xValue - size.X / 2, yValue - size.Y / 2);
        }

        public Point Add(Size size)
        {
            return this.Add(new Vector(size.Width, size.Height));
        }

        public Size GetTotalSize()
        {
            var xValue = organiser.Values.GroupBy(a => a.Key.Item1).Select(a => a.Max(c => c.Value.X)).Sum();
            var yValue = organiser.Values.GroupBy(a => a.Key.Item2).Select(a => a.Max(c => c.Value.Y)).Sum();
            return new Size(xValue, yValue);
        }

        public static int GetCount(int number)
        {
            return (int)Math.Sqrt(number - 1) + 1;
        }
    }

    public class XYOrganiser<T> : IOrganiser<T> where T : struct
    {
        public Dictionary<(int?, int?), T> Values { get; } = new Dictionary<(int?, int?), T>();

        private int j = 0;
        private int x = 0, y = 0;

        public ((int?, T)[], (int?, T)[]) Add(T value)
        {
            var xValue = Values.Where(a => a.Key.Item1 < x).Select(a => (a.Key.Item1, a.Value)).ToArray();
            var yValue = Values.Where(a => a.Key.Item2 < y).Select(a => (a.Key.Item2, a.Value)).ToArray();

            Values.Add((x, y), value);

            if (y == 0)
                y = x = ++j;
            else if (x > 0 && y == j)
                x--;
            else if (y > 0 || x == 0)
            {
                y--;
                x = j;
            }

            return (xValue, yValue);
        }
    }
}