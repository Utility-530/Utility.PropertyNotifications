using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Utility.WPF.Demo.Panels
{
    internal struct Coordinate : IComparable<Coordinate>
    {
        public Coordinate(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public int X { get; }
        public int Y { get; }

        public override string ToString()
        {
            return "X:" + X + " Y: " + Y;
        }

        public static System.Drawing.Size Size(List<Coordinate> coordinates)
        {
            var minX = coordinates.Min(a => a.X);
            var minY = coordinates.Min(a => a.Y);
            var maxX = coordinates.Max(a => a.X);
            var maxY = coordinates.Max(a => a.Y);

            return new System.Drawing.Size(maxX - minX, maxY - minY);
        }

        public static double Size(System.Drawing.Size size)
        {
            return Math.Pow(size.Width, 2) + Math.Pow(size.Height, 2);
        }

        public int CompareTo([AllowNull] Coordinate other)
        {
            return this.X.CompareTo(other.X) + this.Y.CompareTo(other.Y);
        }

        public bool IsDiagonal(Coordinate coordinate)
        {
            return Math.Abs(coordinate.X - this.X) == 1 && Math.Abs(coordinate.Y - this.Y) == 1;
        }

        public bool IsSide(Coordinate coordinate)
        {
            return (Math.Abs(coordinate.X - this.X) == 0 && Math.Abs(coordinate.Y - this.Y) == 1
                || Math.Abs(coordinate.X - this.X) == 1 && Math.Abs(coordinate.Y - this.Y) == 0);
        }

        public bool IsTouching(Coordinate coordinate)
        {
            return Math.Abs(coordinate.Y - this.Y) <= 1 && Math.Abs(coordinate.X - this.X) <= 1;
        }
    }
}